using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SaveWise.BusinessLogic.Common;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;
using SaveWise.DataLayer.Models.Plans;
using SaveWise.DataLayer.Sys.Exceptions;

namespace SaveWise.BusinessLogic.Services
{
    public class PlanService : Service<Plan>, IPlanService
    {
        private readonly PredefinedCategories _predefinedCategories;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IIncomeCategoryService _incomeCategoryService;

        public PlanService(
            IRepositoryFactory repositoryFactory,
            PredefinedCategories predefinedCategories,
            IExpenseCategoryService expenseCategoryService,
            IIncomeCategoryService incomeCategoryService)
            : base(repositoryFactory)
        {
            _predefinedCategories = predefinedCategories;
            _expenseCategoryService = expenseCategoryService;
            _incomeCategoryService = incomeCategoryService;
        }

        public async Task<PlanSummary> GetCurrentPlanSummaryAsync()
        {
            IGenericRepository<Plan> plansRepo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                FilterExpression = GetCurrentPlanCondition(),
                PageSize = 1
            };
            List<Plan> plans = await plansRepo.GetAsync(filter);
            if (plans == null || plans.Count == 0 || plans.Count > 1)
            {
                throw new DocumentNotFoundException("Nie odnaleziono planu budżetowego o wskazanych kryteriach");
            }

            Plan plan = plans.SingleOrDefault();

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono planu budżetowego o wskazanych kryteriach");
            }

            return await GetSummary(plan.Id);
        }

        public override async Task<Plan> GetByIdAsync(string id)
        {
            IGenericRepository<Plan> repo = RepositoryFactory.GetGenericRepository<Plan>();
            var plan = await repo.GetByIdAsync(id);
            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono planu budżetowego o wskazanych kryteriach");
            }

            return plan;
        }

        public override async Task UpdateAsync(string id, Plan document)
        {
            await _incomeCategoryService.InsertManyAsync(document.Incomes?.Select(x => new IncomeCategory
            {
                Name = x.Category
            }));

            IEnumerable<ExpenseCategory> expenseCategories = document
                .Expenses?
                .GroupBy(x => x.Category)
                .Select(y => new ExpenseCategory
            {
                Name = y.Key,
                Types = y.Select(t => new ExpenseType
                {
                    Name = t.Type
                }).ToList()
            });

            await _expenseCategoryService.InsertManyAsync(expenseCategories);

            await base.UpdateAsync(id, document);
        }

        public override async Task InsertAsync(Plan document)
        {
            IGenericRepository<Plan> repo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                FilterExpression = plan => document.StartDate <= plan.StartDate,
                PageSize = 1
            };

            List<Plan> existingCurrentPlans = await repo.GetAsync(filter);
            if (existingCurrentPlans.Any())
            {
                throw new DuplicateNameException("Już istnieje plan budżetowy dla wybranego okresu");
            }

            await _incomeCategoryService.InsertManyAsync(document.Incomes?.Select(x => new IncomeCategory
            {
                Name = x.Category
            }));

            IEnumerable<ExpenseCategory> expenseCategories = document
                .Expenses?
                .GroupBy(x => x.Category)
                .Select(y => new ExpenseCategory
                {
                    Name = y.Key,
                    Types = y.Select(t => new ExpenseType
                    {
                        Name = t.Type
                    }).ToList()
                });

            await _expenseCategoryService.InsertManyAsync(expenseCategories);

            await repo.InsertAsync(document);
        }

        public async Task<Plan> GetNewPlanAsync()
        {
            IGenericRepository<Plan> plansRepo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                PageSize = 1
            };

            Plan lastPlan = (await plansRepo.GetAsync(filter)).FirstOrDefault();

            var plan = new Plan();
            var now = DateTime.Now;
            plan.StartDate = new DateTime(now.Year, now.Month, 1);
            plan.EndDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));

            if (lastPlan?.Incomes != null)
            {
                plan.Incomes = lastPlan.Incomes.Select(item => new Income
                {
                    Category = item.Category,
                }).ToList();
            }
            else
            {
                plan.Incomes = _predefinedCategories.IncomeCategories.Select(x => new Income
                {
                    Category = x.Name
                }).ToList();
            }

            if (lastPlan?.Expenses != null)
            {
                plan.Expenses = lastPlan.Expenses.Select(x => new Expense
                {
                    Type = x.Type,
                    Category = x.Category
                }).ToList();
            }
            else
            {
                plan.Expenses = (from expenseCategory in _predefinedCategories.ExpenseCategories
                    from expenseCategoryType in expenseCategory.Types
                    select new Expense
                    {
                        Type = expenseCategoryType.Name,
                        Category = expenseCategory.Name
                    }).ToList();
            }

            return plan;
        }

        public async Task<IList<Income>> GetPlanIncomes(string planId)
        {
            IGenericRepository<Plan> repo = RepositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await repo.GetByIdAsync(planId);

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono wskazanego budżetu w bazie danych");
            }

            return plan.Incomes;
        }

        public async Task UpdatePlanIncomes(string planId, IList<Income> incomes)
        {
            await ExecuteWithPlan(planId, async (plan, repo) =>
            {
                plan.Incomes = incomes;
                await repo.UpdateAsync(planId, plan);
            });
        }

        public async Task<PlanSummary> GetSummary(string planId)
        {
            return await ExecuteWithPlan(planId, async (plan, repository) =>
            {
                return await Task.Factory.StartNew(() => new PlanSummary
                {
                    Id = plan.Id,
                    StartDate = plan.StartDate,
                    EndDate = plan.EndDate,
                    IncomesSum = plan.Incomes?.Sum(income => income.Amount) ?? 0.0f,
                    ExpensesSum = plan.Expenses?.Sum(expense => expense.Amount) ?? 0.0f,
                    ExpensesPerCategory = plan.Expenses?
                        .GroupBy(key => key.Category)
                        .Select(item => new SumPerCategory
                        {
                            Category = item.Key,
                            Sum = item.Sum(x => x.Amount),
                            PlannedSum = item.Sum(x => x.PlannedAmount)
                        }).ToList(),
                    IncomesPerCategory = plan.Incomes?
                        .GroupBy(key => key.Category)
                        .Select(item => new SumPerCategory
                        {
                            Category = item.Key,
                            Sum = item.Sum(x => x.Amount),
                            PlannedSum = item.Sum(x => x.PlannedAmount)
                        }).ToList()
                });
            });
        }

        private async Task ExecuteWithPlan(string planId, Func<Plan, IGenericRepository<Plan>, Task> func)
        {
            IGenericRepository<Plan> repo = RepositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await repo.GetByIdAsync(planId);

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono wskazanego budżetu w bazie danych");
            }

            await func(plan, repo);
        }

        private async Task<T> ExecuteWithPlan<T>(string planId, Func<Plan, IGenericRepository<Plan>, Task<T>> func)
        {
            IGenericRepository<Plan> repo = RepositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await repo.GetByIdAsync(planId);

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono wskazanego budżetu w bazie danych");
            }

            return await func(plan, repo);
        }

        private static Expression<Func<Plan, bool>> GetCurrentPlanCondition()
        {
            DateTime now = DateTime.Today;
            return plan => plan.StartDate <= now && plan.EndDate >= now;
        }
    }
}