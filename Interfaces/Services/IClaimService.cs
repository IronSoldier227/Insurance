using Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IClaimService
    {
        Task<int> CreateClaimAsync(Claim claimDto);
        Task<IEnumerable<Claim>> GetClientClaimsAsync(int clientId);
        Task<IEnumerable<Claim>> GetByClientIdAsync(int clientId);
        Task<Claim?> GetClaimByIdAsync(int id);
        Task DecideClaimAsync(int claimId, int managerId, double? payoutAmount);
    }
}
