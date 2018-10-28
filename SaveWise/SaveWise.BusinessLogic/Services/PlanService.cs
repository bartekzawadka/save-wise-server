using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public class PlanService : Service<Plan>
    {
        public PlanService(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }
    }
}