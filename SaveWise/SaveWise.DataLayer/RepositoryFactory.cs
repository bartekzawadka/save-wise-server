using System.Collections.Concurrent;
using SaveWise.DataLayer.Models;

namespace SaveWise.DataLayer
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ISaveWiseContext _context;
        private readonly ConcurrentDictionary<string, object> _repositoriesDictionary = new ConcurrentDictionary<string, object>();

        public RepositoryFactory(ISaveWiseContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : Document
        {
            var repo = _repositoriesDictionary.GetOrAdd(nameof(T), new GenericRepository<T>(_context));
            if (repo is IGenericRepository<T> genericRepository)
            {
                return genericRepository;
            }

            return null;
        }
    }
}