using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IPolicyRepository
    {
        Task<InsurancePolicy?> GetByIdAsync(int id);
        Task<IEnumerable<InsurancePolicy>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<InsurancePolicy>> GetPoliciesByVehicleIdAsync(int vehicleId);
        Task AddAsync(InsurancePolicy policy);
        void Update(InsurancePolicy policy);

        Task<IEnumerable<InsurancePolicy>> GetAllWithIncludesAsync(params Expression<Func<InsurancePolicy, object>>[] includes);
    }
}