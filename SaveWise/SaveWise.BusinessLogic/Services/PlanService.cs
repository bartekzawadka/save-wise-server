using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;
using SaveWise.DataLayer.Sys;

namespace SaveWise.BusinessLogic.Services
{
    public class PlanService : Service<Plan>, IPlanService
    {
        public PlanService(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public async Task<IList<IncomeCategory>> GetNewPlanIncomeCategoriesAsync()
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
            if (lastPlan == null || lastPlan.PlannedIncomes?.Any() != true)
            {
                return GetPredefinedIncomeCategories();
            }

            return lastPlan.PlannedIncomes.Select(income => income.Category).ToList();
        }

        private static IList<IncomeCategory> GetPredefinedIncomeCategories()
        {
            return new List<IncomeCategory>
            {
                new IncomeCategory
                {
                    Name = "Wynagrodzenie"
                },
                new IncomeCategory
                {
                    Name = "Wynagrodzenie Partnera / Partnerki"
                },
                new IncomeCategory
                {
                    Name = "Premia"
                },
                new IncomeCategory
                {
                    Name = "Inne przychody"
                },
            };
        }
    }
}