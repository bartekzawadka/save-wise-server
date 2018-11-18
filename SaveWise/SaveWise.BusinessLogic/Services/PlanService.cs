using System.Collections.Generic;
using System.Linq;
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

        public PlanService(
            IRepositoryFactory repositoryFactory,
            PredefinedCategories predefinedCategories)
            : base(repositoryFactory)
        {
            _predefinedCategories = predefinedCategories;
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
                }
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
    }
}