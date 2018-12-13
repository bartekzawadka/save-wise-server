using System;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
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

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new AuthenticationException("Username is required");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AuthenticationException("Password is required");
            }

            var user = await _userRepository.GetByNameAsync(username);
            if (user == null)
            {
                throw new AuthenticationException($"User '{username}' could not be found");
            }

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new AuthenticationException("Invalid password");
            }

            return user;
        }

        public async Task CreateAsync(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                throw new ArgumentNullException(nameof(user), "Missing account parameters");
            }

            if (await _userRepository.GetUserExists(user.Username))
            {
                throw new DuplicateNameException($"User '{user.Username}' already exists");
            }
            
            CreatePasswordHash(user);

            await _userRepository.InsertAsync(user);
        }

        public Task DeleteAsync(string id)
        {
            return _userRepository.DeleteAsync(id);
        }

        public async Task ChangePassword(string username, string password, string passwordConfirm)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(passwordConfirm))
            {
                throw new ArgumentNullException("Missing parameters", new Exception());
            }

            if (!string.Equals(password, passwordConfirm))
            {
                throw new AuthenticationException("New password does not match confirmed value");
            }

            var user = await _userRepository.GetByNameAsync(username);
            if (user == null)
            {
                throw new AuthenticationException($"User '{username}' could not be found or more than one such users found");
            }

            user.Password = password;
            CreatePasswordHash(user);

            await _userRepository.UpdateAsync(user.Id, user);
        }
        
        private static void CreatePasswordHash(User user)
        { 
            using (var hmac = new HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password));
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
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                if (computedHash.Where((t, i) => t != storedHash[i]).Any())
                {
                    return false;
                }
            }
 
            return true;
        }
    }
}