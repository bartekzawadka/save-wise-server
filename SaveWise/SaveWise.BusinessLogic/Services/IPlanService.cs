using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Plans;

namespace SaveWise.BusinessLogic.Services
{
    public interface IPlanService : IService<Plan>
    {
        Task<NewPlan> GetNewPlanAsync();
    }
}