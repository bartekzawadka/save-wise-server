using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Plans;

namespace SaveWise.BusinessLogic.Services
{
    public interface IPlanService : IService<Plan>
    {
        Task<PlanSummary> GetCurrentPlanAsync();

        Task<NewPlan> GetNewPlanAsync();

        Task<IList<Income>> GetPlanIncomes(string planId);

        Task UpdatePlanIncomes(string planId, IList<Income> incomes);

        Task<PlanSummary> GetSummary(string planId);
    }
}