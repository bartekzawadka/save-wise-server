using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.BusinessLogic.Services
{
    public class Service<TDocument> : IService<TDocument> where TDocument : Document
    {
        protected readonly IRepositoryFactory RepositoryFactory;

        public Service(IRepositoryFactory repositoryFactory)
        {
            RepositoryFactory = repositoryFactory;
        }
        
        public virtual Task<List<TDocument>> GetAsync<TFilter>(TFilter filter) 
            where TFilter : Filter<TDocument>, new()
        {
            return RepositoryFactory.GetGenericRepository<TDocument>().GetAsync(filter);
        }

        public virtual Task<TDocument> GetByIdAsync(string id)
        {
            return RepositoryFactory.GetGenericRepository<TDocument>().GetByIdAsync(id);
        }

        public virtual Task InsertAsync(TDocument document)
        {
            return RepositoryFactory.GetGenericRepository<TDocument>().InsertAsync(document);
        }

        public virtual Task InsertManyAsync(IEnumerable<TDocument> documents)
        {
            return RepositoryFactory.GetGenericRepository<TDocument>().InsertManyAsync(documents);
        }

        public virtual Task UpdateAsync(string id, TDocument document)
        {
            return RepositoryFactory.GetGenericRepository<TDocument>().UpdateAsync(id, document);
        }

        public virtual Task<bool> DeleteAsync(string id)
        {
            return RepositoryFactory.GetGenericRepository<TDocument>().DeleteAsync(id);
        }
    }
}