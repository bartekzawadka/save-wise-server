using SaveWise.DataLayer.Models;

namespace SaveWise.DataLayer
{
    public interface IRepositoryFactory
    {
        IGenericRepository<T> GetGenericRepository<T>() where T : Document;
    }
}