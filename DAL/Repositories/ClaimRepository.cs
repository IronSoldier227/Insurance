using DAL.Context;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Repository;

namespace DAL.Repositories
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly InsuranceDbContext _context;

        public ClaimRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InsuranceClaim>> GetAllWithRelatedDataAsync()
        {
            return await _context.InsuranceClaims
                .Include(c => c.Policy) // Подгружаем связанный полис
                    .ThenInclude(p => p.Vehicle) // Подгружаем машину из полиса
                        .ThenInclude(v => v.Model) // Подгружаем модель машины
                            .ThenInclude(m => m.Brand) // Подгружаем марку машины
                .Include(c => c.Status) // Подгружаем связанный статус
                .ToListAsync();
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

        public async Task<IEnumerable<InsuranceClaim>> GetAllWithPolicyAndVehicleAsync()
        {
            return await _context.InsuranceClaims
                .Include(c => c.Policy).ThenInclude(p => p.Vehicle).ToListAsync();
        }
    }
}
