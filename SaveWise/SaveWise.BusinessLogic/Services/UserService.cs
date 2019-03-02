using System;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Users;
using SaveWise.DataLayer.User;

namespace SaveWise.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetById(string id)
        {
            return _userRepository.GetById(id);
        }

        public Task<User> GetByIdAsync(string id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new AuthenticationException("Nazwa użytkownika jest wymagana");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AuthenticationException("Hasło jest wymagane");
            }

            User user = await _userRepository.GetByNameAsync(username);
            if (user == null)
            {
                throw new AuthenticationException("Nieprawidłowa nazwa użytkownika lub hasło");
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new AuthenticationException("Nieprawidłowa nazwa użytkownika lub hasło");
            }

            return user;
        }

        public async Task<User> CreateAsync(Register registration)
        {
            if (registration == null || string.IsNullOrWhiteSpace(registration.Username) || string.IsNullOrWhiteSpace(registration.Password))
            {
                throw new ArgumentNullException(nameof(registration), "Brak danych do rejestracji");
            }

            if (!string.Equals(registration.Password, registration.PasswordConfirm))
            {
                throw new AuthenticationException("Potwierdzenie hasła różni się od oryginału");
            }

            if (await _userRepository.GetUserExists(registration.Username))
            {
                throw new DuplicateNameException($"Użytkownik '{registration.Username}' już istnieje");
            }

            var user = new User
            {
                Password = registration.Password,
                Username = registration.Username
            };

            CreatePasswordHash(user);

            await _userRepository.InsertAsync(user);
            return user;
        }

        public Task DeleteAsync(string id)
        {
            return _userRepository.DeleteAsync(id);
        }

        public async Task ChangePassword(ChangePassword changePassword)
        {
            if (string.IsNullOrWhiteSpace(changePassword.Username)
                || string.IsNullOrWhiteSpace(changePassword.OldPassword)
                || string.IsNullOrWhiteSpace(changePassword.NewPassword)
                || string.IsNullOrWhiteSpace(changePassword.PasswordConfirm))
            {
                throw new ArgumentNullException("Brak wymaganych parametrów zmiany hasła", new Exception());
            }

            if (!string.Equals(changePassword.NewPassword, changePassword.PasswordConfirm))
            {
                throw new AuthenticationException("Potwierdzenie hasła różni się od oryginału");
            }

            User user = await _userRepository.GetByNameAsync(changePassword.Username);
            if (user == null)
            {
                throw new AuthenticationException($"Użytkownik '{changePassword.Username}' nie istnieje");
            }

            if (!VerifyPasswordHash(changePassword.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                throw new AuthenticationException("Nieprawidłowe hasło");
            }

            user.Password = changePassword.NewPassword;
            CreatePasswordHash(user);

            await _userRepository.UpdateAsync(user.Id, user);
        }

        private static void CreatePasswordHash(User user)
        {
            using (var hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));
            }

            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
            }

            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedHash));
            }

            using (var hmac = new HMACSHA512(storedSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                if (computedHash.Where((t, i) => t != storedHash[i]).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}