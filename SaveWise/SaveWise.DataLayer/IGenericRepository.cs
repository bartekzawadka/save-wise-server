using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.DataLayer
{
    public interface IGenericRepository<TCollection> where TCollection : Document
    {
        string CollectionName { get; }

        Task<List<TCollection>> GetAsync<TFilter>(TFilter filter = null)
            where TFilter : Filter<TCollection>, new();

        Task<TCollection> GetByIdAsync(string id);

        Task InsertAsync(TCollection document);

        Task InsertManyAsync(IEnumerable<TCollection> documents);

        Task UpdateAsync(string id, TCollection document);

        Task<bool> DeleteAsync(string id);

        Task<bool> DeleteManyAsync(IEnumerable<string> ids);
    }
}