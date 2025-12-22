using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Interfaces.Repository
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetByClientIdAsync(int clientId);
        Task<Vehicle?> GetByIdAsync(int id);
        void Update(Vehicle vehicle);
        void Remove(Vehicle vehicle);
        Task<int> SaveChangesAsync();
    }
}