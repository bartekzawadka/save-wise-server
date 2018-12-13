using System.Threading.Tasks;
using MongoDB.Driver;

namespace SaveWise.DataLayer.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ISaveWiseContext _context;
        private readonly string _collectionName = nameof(User).ToLowerInvariant();

        public UserRepository(ISaveWiseContext context)
        {
            _context = context;
        }

        public Models.User GetById(string id)
        {
            return GetUserCollection().Find(user => string.Equals(user.Id, id)).SingleOrDefault();
        }
        
        public async Task<Models.User> GetByIdAsync(string id)
        {
            var result = await GetUserCollection().FindAsync(user => string.Equals(user.Id, id));
            return await result.SingleOrDefaultAsync();
        }

        public async Task<Models.User> GetByNameAsync(string username)
        {
            var result = await GetUserCollection().FindAsync(user => string.Equals(user.Username, username));
            return await result.SingleOrDefaultAsync();
        }

        public async Task<bool> GetUserExists(string username)
        {
            var users = await GetUserCollection().FindAsync(user => string.Equals(user.Username, username));
            return users?.Any() == true;
        }
        
        public Task InsertAsync(Models.User user)
        {
            return GetUserCollection().InsertOneAsync(user);
        }

        public Task UpdateAsync(string id, Models.User user)
        {
            return GetUserCollection().FindOneAndReplaceAsync(i => string.Equals(i.Id, id), user);
        }

        public Task DeleteAsync(string id)
        {
            return GetUserCollection().DeleteOneAsync(user => string.Equals(user.Id, id));
        }

        private IMongoCollection<Models.User> GetUserCollection()
        {
            return _context.Database.GetCollection<Models.User>(_collectionName);
        }
    }
}