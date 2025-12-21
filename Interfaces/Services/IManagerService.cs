using Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IManagerService
    {
        Task<IEnumerable<Client>> SearchClientsAsync(string? name, string? phone, string? policyNumber);
        Task<ClientDetails?> GetClientDetailsAsync(int clientId);
        Task CancelPolicyAsync(int policyId, int managerId);
        Task DecideClaimAsync(int claimId, int managerId, double? payoutAmount);
    }
}
