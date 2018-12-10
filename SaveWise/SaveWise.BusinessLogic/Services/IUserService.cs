using System.Threading.Tasks;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public interface IUserService
    {
        User GetById(string id);
        
        Task<User> AuthenticateAsync(string username, string password);

        Task<User> CreateAsync(User user);

        Task<bool> DeleteAsync(string id);

        Task ChangePassword(string username, string password, string passwordConfirm);
    }
}