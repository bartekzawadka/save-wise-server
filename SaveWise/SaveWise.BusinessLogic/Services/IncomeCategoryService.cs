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
            var repo = RepositoryFactory.GetGenericRepository<IncomeCategory>();
            
            var categories = await repo.GetAsync<Filter<IncomeCategory>>();
            var categoryNames = categories.Select(c => c.Name.ToLower().Trim()).ToList();

            var missingCategories = documents.Where(c => !categoryNames.Contains(c.Name.ToLower().Trim())).ToList();

            await repo.InsertManyAsync(missingCategories);
        }
    }
}