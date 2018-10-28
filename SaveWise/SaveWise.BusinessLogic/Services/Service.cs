using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.BusinessLogic.Services
{
    public class Service<TDocument> : IService<TDocument> where TDocument : Document
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public Service(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }
        
        public virtual Task<List<TDocument>> GetAsync<TFilter>(TFilter filter) 
            where TFilter : Filter<TDocument>, new()
        {
            return _repositoryFactory.GetGenericRepository<TDocument>().GetAsync(filter);
        }

        public virtual Task<TDocument> GetByIdAsync(string id)
        {
            return _repositoryFactory.GetGenericRepository<TDocument>().GetByIdAsync(id);
        }

        public virtual Task InsertAsync(TDocument document)
        {
            return _repositoryFactory.GetGenericRepository<TDocument>().InsertAsync(document);
        }

        public virtual Task UpdateAsync(string id, TDocument document)
        {
            return _repositoryFactory.GetGenericRepository<TDocument>().UpdateAsync(id, document);
        }

        public virtual Task<bool> DeleteAsync(string id)
        {
            return _repositoryFactory.GetGenericRepository<TDocument>().DeleteAsync(id);
        }
    }
}