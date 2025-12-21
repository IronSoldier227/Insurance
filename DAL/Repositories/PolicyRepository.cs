using Core.Entities;
using DAL.Context;
using Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class PolicyRepository : Interfaces.Repository.IPolicyRepository
    {
        private readonly InsuranceDbContext _context;

        public PolicyRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<InsurancePolicy?> GetByIdAsync(int id)
        {
            return await _context.InsurancePolicies
                .Include(p => p.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<InsurancePolicy>> GetByClientIdAsync(int clientId)
        {
            return await _context.InsurancePolicies
                .Include(p => p.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Brand)
                .Where(p => p.Vehicle.ClientId == clientId).ToListAsync();
        }

        // --- Новая реализация ---
        public async Task<IEnumerable<InsurancePolicy>> GetPoliciesByVehicleIdAsync(int vehicleId)
        {
            return await _context.InsurancePolicies
                .Where(p => p.VehicleId == vehicleId) // <-- Фильтруем по VehicleId
                .ToListAsync();
        }

        public async Task AddAsync(InsurancePolicy policy)
        {
            await _context.InsurancePolicies.AddAsync(policy);
            await _context.SaveChangesAsync();
        }

        public void Update(InsurancePolicy policy)
        {
            _context.InsurancePolicies.Update(policy);
            _context.SaveChanges();
        }
    }
}