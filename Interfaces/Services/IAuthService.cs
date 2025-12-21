using System.Threading.Tasks;
using Interfaces.DTO;

namespace Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserDto?> AuthenticateAsync(string login, string password);
    }
}
