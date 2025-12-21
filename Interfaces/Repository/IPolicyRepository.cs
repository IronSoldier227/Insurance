using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IPolicyRepository
    {
        Task<InsurancePolicy?> GetByIdAsync(int id);
        Task<IEnumerable<InsurancePolicy>> GetByClientIdAsync(int clientId);
        // --- Новый метод ---
        Task<IEnumerable<InsurancePolicy>> GetPoliciesByVehicleIdAsync(int vehicleId); // <-- Добавляем
        Task AddAsync(InsurancePolicy policy);
        void Update(InsurancePolicy policy);
    }
}