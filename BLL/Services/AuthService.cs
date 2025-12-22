using Interfaces.Services;
using Interfaces.DTO;
using Interfaces.Repository;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly Interfaces.Repository.IUserRepository _userRepository;

        public AuthService(Interfaces.Repository.IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> AuthenticateAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null) return null;

            var inputHash = ComputeHash(password); 
            var storedHash = Encoding.UTF8.GetString(user.PasswordHash);

            if (inputHash != storedHash) return null;

            return new UserDto { Id = user.Id, Login = user.Login, IsClient = user.IsClient };
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
