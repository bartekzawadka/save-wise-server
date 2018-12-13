using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.DataLayer
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ISaveWiseContext _context;
        private readonly IIdentityProvider _identityProvider;
        private readonly ConcurrentDictionary<string, object> _repositoriesDictionary = new ConcurrentDictionary<string, object>();

        public RepositoryFactory(ISaveWiseContext context, IIdentityProvider identityProvider)
        {
            _context = context;
            _identityProvider = identityProvider;
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : Document
        {
            var name = Regex.Replace(typeof(T).Name, "(\\B[A-Z])", ".$1").ToLower();
            
            var repo = _repositoriesDictionary.GetOrAdd(
                name,
                new GenericRepository<T>(_context, _identityProvider, name));
            
            if (repo is IGenericRepository<T> genericRepository)
            {
                return genericRepository;
            }

            return null;
        }
    }
}