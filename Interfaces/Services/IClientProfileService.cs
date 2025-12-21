using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IClientProfileService
    {
        Task<ClientProfileDto?> GetByUserIdAsync(int userId);
    }
}