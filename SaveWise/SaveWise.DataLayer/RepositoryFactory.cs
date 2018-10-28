using System.Collections.Concurrent;
using System.Text.RegularExpressions;
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
            var name = Regex.Replace(typeof(T).Name, "(\\B[A-Z])", ".$1").ToLower();
            
            var repo = _repositoriesDictionary.GetOrAdd(name, new GenericRepository<T>(_context, name));
            if (repo is IGenericRepository<T> genericRepository)
            {
                return genericRepository;
            }

            return null;
        }
    }
}