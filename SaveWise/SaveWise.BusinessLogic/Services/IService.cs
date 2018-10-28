using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.BusinessLogic.Services
{
    public interface IService<TDocument> where TDocument : Document
    {
        Task<List<TDocument>> GetAsync<TFilter>(TFilter filter)
            where TFilter : Filter<TDocument>, new();

        Task<TDocument> GetByIdAsync(string id);

        Task InsertAsync(TDocument document);

        Task UpdateAsync(string id, TDocument document);

        Task<bool> DeleteAsync(string id);
    }
}