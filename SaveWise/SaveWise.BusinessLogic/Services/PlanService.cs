using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using SaveWise.BusinessLogic.Common;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;
using SaveWise.DataLayer.Models.Plans;
using SaveWise.DataLayer.Sys;
using SaveWise.DataLayer.Sys.Exceptions;

namespace SaveWise.BusinessLogic.Services
{
    public class PlanService : Service<Plan>, IPlanService
    {
        private readonly PredefinedCategories _predefinedCategories;
        private readonly IExpenseCategoryService _expenseCategoryService;

        public PlanService(
            IRepositoryFactory repositoryFactory,
            PredefinedCategories predefinedCategories,
            IExpenseCategoryService expenseCategoryService)
            : base(repositoryFactory)
        {
            _predefinedCategories = predefinedCategories;
            _expenseCategoryService = expenseCategoryService;
        }

        public async Task<PlanSummary> GetCurrentPlanAsync()
        {
            var plansRepo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                FilterExpression = GetCurrentPlanCondition(),
                PageSize = 1
            };
            var plans = await plansRepo.GetAsync(filter);
            var plan = plans.SingleOrDefault();

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono planu budżetowego o wskazanych kryteriach");
            }

            return await GetSummary(plan.Id);
        }

        public override async Task InsertAsync(Plan document)
        {
            var repo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                FilterExpression = plan => document.StartDate <= plan.StartDate,
                PageSize = 1
            };

            var existingCurrentPlans = await repo.GetAsync(filter);
            if (existingCurrentPlans.Any())
            {
                throw new DuplicateNameException("Już istnieje plan budżetowy dla wybranego okresu");
            }

            var expenseCategories = await _expenseCategoryService.GetAsync<Filter<ExpenseCategory>>(null);
            var expenseCategoriesDict = expenseCategories
                .ToDictionary(category => category.Name, category => category.Types);
            var expenseCategoriesNames = expenseCategories.Select(ec => ec.Name).ToList();

            var plannedExpensesDict = document.Expenses.GroupBy(x => x.Category)
                .ToDictionary(x => x.Key, x => x.Select(y => new ExpenseType
                {
                    Name = y.Type
                }).ToList());

            var newCategories = new List<ExpenseCategory>();

            foreach (var plannedExpense in plannedExpensesDict)
            {
                if (!expenseCategoriesNames.Contains(plannedExpense.Key))
                {
                    newCategories.Add(new ExpenseCategory
                    {
                        Name = plannedExpense.Key,
                        Types = plannedExpense.Value
                    });
                }
                else
                {
                    var existingsNames = expenseCategoriesDict[plannedExpense.Key].Select(x => x.Name).ToList();
                    var toBeAdded = plannedExpense.Value.Where(x => !existingsNames.Contains(x.Name)).ToList();

                    if (toBeAdded.Count == 0)
                    {
                        continue;
                    }

                    var dbTypes = (await _expenseCategoryService.GetAsync(new Filter<ExpenseCategory>
                    {
                        FilterExpression = f => string.Equals(f.Name, plannedExpense.Key)
                    })).SingleOrDefault();

                    if (dbTypes == null)
                    {
                        throw new DocumentNotFoundException($"Nie odnaleziono kategorii wydatków o nazwie '{plannedExpense.Key}'");
                    }

                    var types = dbTypes.Types.ToList();

                    types.AddRange(toBeAdded);

                    dbTypes.Types = types;

                    await _expenseCategoryService.UpdateAsync(dbTypes.Id, dbTypes);
                }
            }

            if (newCategories.Count > 0)
            {
                await _expenseCategoryService.InsertManyAsync(newCategories);
            }

            await repo.InsertAsync(document);
        }

        public async Task<NewPlan> GetNewPlanAsync()
        {
            var plansRepo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                PageSize = 1
            };

            var lastPlan = (await plansRepo.GetAsync(filter)).FirstOrDefault();

            var incomeCategories = (lastPlan == null || lastPlan.Incomes?.Any() != true)
                ? _predefinedCategories.IncomeCategories
                : lastPlan.Incomes.Select(income => new IncomeCategory
                {
                    Name = income.Category
                }).ToList();

            var expenseCategories = (lastPlan == null || lastPlan.Expenses?.Any() != true)
                ? _predefinedCategories.ExpenseCategories
                : lastPlan.Expenses.GroupBy(x => x.Category).Select(item =>
                {
                    return new ExpenseCategory
                    {
                        Name = item.Key,
                        Types = item.Select(x => new ExpenseType
                        {
                            Name = x.Type
                        })
                    };
                });

            return new NewPlan
            {
                IncomeCategories = incomeCategories,
                ExpenseCategories = expenseCategories
            };
        }

        public async Task<IList<Income>> GetPlanIncomes(string planId)
        {
            var repo = RepositoryFactory.GetGenericRepository<Plan>();
            var plan = await repo.GetByIdAsync(planId);

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
            var repo = RepositoryFactory.GetGenericRepository<Plan>();
            var plan = await repo.GetByIdAsync(planId);

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono wskazanego budżetu w bazie danych");
            }

            await func(plan, repo);
        }

        private async Task<T> ExecuteWithPlan<T>(string planId, Func<Plan, IGenericRepository<Plan>, Task<T>> func)
        {
            var repo = RepositoryFactory.GetGenericRepository<Plan>();
            var plan = await repo.GetByIdAsync(planId);

            if (plan == null)
            {
                throw new DocumentNotFoundException("Nie odnaleziono wskazanego budżetu w bazie danych");
            }

            return await func(plan, repo);
        }
        
        private static Expression<Func<Plan, bool>> GetCurrentPlanCondition()
        {
            var now = DateTime.Today;
            return plan => plan.StartDate <= now && plan.EndDate > now;
        }
    }
}