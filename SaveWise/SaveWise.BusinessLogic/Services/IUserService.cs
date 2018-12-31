using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Users;

namespace SaveWise.BusinessLogic.Services
{
    public interface IUserService
    {
        User GetById(string id);

        Task<User> AuthenticateAsync(string username, string password);

        Task<User> CreateAsync(Register registration);

        Task DeleteAsync(string id);

        Task ChangePassword(ChangePassword changePassword);
    }
}