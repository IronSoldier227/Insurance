using Core.Entities;
using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface ITypeOfPolicyRepository
    {
        Task<IEnumerable<TypeOfPolicy>> GetAllAsync();
    }
}
