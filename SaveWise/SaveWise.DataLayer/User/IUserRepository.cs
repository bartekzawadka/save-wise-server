using System.Threading.Tasks;

namespace SaveWise.DataLayer.User
{
    public interface IUserRepository
    {
        Models.User GetById(string id);
        
        Task<Models.User> GetByIdAsync(string id);

        Task<Models.User> GetByNameAsync(string username);

        Task<bool> GetUserExists(string username);

        Task InsertAsync(Models.User user);

        Task UpdateAsync(string id, Models.User user);

        Task DeleteAsync(string id);
    }
}