using Core.Entities;
using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<UserDto?> GetByLoginAsync(string login);
        Task<User?> GetByIdAsync(int id);
    }
}
