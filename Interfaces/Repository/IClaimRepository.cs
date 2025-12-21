using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IClaimRepository
    {
        Task<IEnumerable<InsuranceClaim>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<InsuranceClaim>> GetAllWithRelatedDataAsync(); 
        Task<InsuranceClaim?> GetByIdAsync(int id);
        Task AddAsync(InsuranceClaim claim);
        void Update(InsuranceClaim claim);
        Task<IEnumerable<InsuranceClaim>> GetAllWithPolicyAndVehicleAsync();
    }
}
