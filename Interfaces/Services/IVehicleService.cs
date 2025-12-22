using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.DTO;

namespace Interfaces.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetVehiclesByClientIdAsync(int clientId);
        Task<VehicleDto?> GetVehicleByIdAsync(int id);
        Task<int> AddVehicleAsync(VehicleCreateDto dto);
        Task UpdateVehicleAsync(VehicleUpdateDto dto);
        Task DeleteVehicleAsync(int id);
        Task<bool> CanInsureVehicleAsync(int vehicleId);
    }
}