using Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IManagerRepository
    {
        Task<IEnumerable<Client>> SearchClientsAsync(string? name, string? phone, string? policyNumber);
        Task<ClientDetails?> GetClientDetailsAsync(int clientId);
    }
}
