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
using SaveWise.DataLayer.Sys;

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

        public async Task<Plan> GetCurrentPlanAsync()
        {
            var plansRepo = RepositoryFactory.GetGenericRepository<Plan>();
            var filter = new PlansFilter
            {
                FilterExpression = GetCurrentPlanCondition(),
                Sorting = new List<ColumnSort>
                {
                    new ColumnSort
                    {
                        ColumnName = nameof(Plan.PlannedIncomes),
                        IsDescending = true
                    }
                },
                PageSize = 1
            };
            var plans = await plansRepo.GetAsync(filter);
            return plans.SingleOrDefault();
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

            var plannedExpensesDict = document.PlannedExpenses.GroupBy(x => x.Category)
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
                        throw new Exception($"Nie odnaleziono kategorii wydatków o nazwie '{plannedExpense.Key}'");
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
                Sorting = new List<ColumnSort>
                {
                    new ColumnSort
                    {
                        ColumnName = nameof(Plan.PlannedIncomes),
                        IsDescending = true
                    }
                },
                PageSize = 1
            };

            var lastPlan = (await plansRepo.GetAsync(filter)).FirstOrDefault();

            var incomeCategories = (lastPlan == null || lastPlan.PlannedIncomes?.Any() != true)
                ? _predefinedCategories.IncomeCategories
                : lastPlan.PlannedIncomes.Select(income => new IncomeCategory
                {
                    Name = income.Category
                }).ToList();

            var expenseCategories = (lastPlan == null || lastPlan.PlannedExpenses?.Any() != true)
                ? _predefinedCategories.ExpenseCategories
                : lastPlan.PlannedExpenses.GroupBy(x => x.Category).Select(item =>
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

        private static Expression<Func<Plan, bool>> GetCurrentPlanCondition()
        {
            var now = DateTime.Today;
            return plan => plan.StartDate <= now && plan.EndDate > now;
        }
    }
}