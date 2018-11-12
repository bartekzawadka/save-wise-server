using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public interface IPlanService : IService<Plan>
    {
        Task<IList<IncomeCategory>> GetNewPlanIncomeCategoriesAsync();
    }
}