using Core.Entities;
using Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IPolicyService
    {
        Task<int> CreatePolicyAsync(Insurance policyDto);
        Task<IEnumerable<Insurance>> GetClientPoliciesAsync(int clientId);
        Task<Insurance?> GetPolicyByIdAsync(int id);
        Task CancelPolicyAsync(int policyId, int cancelledByManagerId);
        Task<IEnumerable<Insurance>> GetPoliciesByVehicleId(int vehicleId);
        Task<IEnumerable<Insurance>> GetByClientIdAsync(int userId);
        Task<AnnualPolicyRevenueReportDto> GetAnnualRevenueReportAsync(int year);
        Task<IEnumerable<Insurance>> GetActivePoliciesByVehicleIdAsync(int vehicleId); 

    }
}
