using DAL.Context;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ClaimRepository : Interfaces.Repository.IClaimRepository
    {
        private readonly InsuranceDbContext _context;

        public ClaimRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InsuranceClaim claim)
        {
            await _context.InsuranceClaims.AddAsync(claim);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InsuranceClaim>> GetByClientIdAsync(int clientId)
        {
            return await _context.InsuranceClaims
                .Include(c => c.Policy).ThenInclude(p => p.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Brand)
                .Where(c => c.Policy.Vehicle.ClientId == clientId).ToListAsync();
        }

        public async Task<InsuranceClaim?> GetByIdAsync(int id)
        {
            return await _context.InsuranceClaims
                .Include(c => c.Policy).ThenInclude(p => p.Vehicle).ThenInclude(v => v.Model).ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(InsuranceClaim claim)
        {
            _context.InsuranceClaims.Update(claim);
            _context.SaveChanges();
        }
    }
}
