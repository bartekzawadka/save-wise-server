using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;
using SaveWise.DataLayer.Models.Plans;

namespace SaveWise.BusinessLogic.Services
{
    public interface IPlanService : IService<Plan>
    {
        Task<IList<PlanListItem>> GetAsync(PlansFilter filter);
        
        Task<PlanSummary> GetCurrentPlanSummaryAsync();

        Task<Plan> GetNewPlanAsync();

        Task<IList<Income>> GetPlanIncomesAsync(string planId);

        Task UpdatePlanIncomesAsync(string planId, IList<Income> incomes);

        Task<PlanSummary> GetSummaryAsync(string planId);
    }
}