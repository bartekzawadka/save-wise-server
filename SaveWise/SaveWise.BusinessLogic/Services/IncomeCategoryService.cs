using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.BusinessLogic.Services
{
    public class IncomeCategoryService : Service<IncomeCategory>, IIncomeCategoryService
    {
        public IncomeCategoryService(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }
        
        public override async Task InsertManyAsync(IEnumerable<IncomeCategory> documents)
        {
            if (documents == null)
            {
                return;
            }
            
            IGenericRepository<IncomeCategory> repo = RepositoryFactory.GetGenericRepository<IncomeCategory>();

            List<IncomeCategory> categories = await repo.GetAsync<Filter<IncomeCategory>>();
            List<string> categoryNames = categories.Select(c => c.Name.ToLower().Trim()).ToList();

            List<IncomeCategory> missingCategories = documents
                .Where(c => !categoryNames.Contains(c.Name.ToLower().Trim()))
                .ToList();

            if (missingCategories.Count > 0)
            {
                await repo.InsertManyAsync(missingCategories);
            }
        }
    }
}