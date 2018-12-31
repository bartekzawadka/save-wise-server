using System.Threading.Tasks;

namespace SaveWise.DataLayer.User
{
    public interface IUserRepository
    {
        Models.Users.User GetById(string id);

        Task<Models.Users.User> GetByIdAsync(string id);

        Task<Models.Users.User> GetByNameAsync(string username);

        Task<bool> GetUserExists(string username);

        Task InsertAsync(Models.Users.User user);

        Task UpdateAsync(string id, Models.Users.User user);

        Task DeleteAsync(string id);
    }
}