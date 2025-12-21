using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto?> AuthenticateAsync(string login, string password);
        Task<int> RegisterAsync(UserCreateDto dto);
        Task<UserDto?> GetByIdAsync(int id);
    }
}
