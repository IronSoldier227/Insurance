using DAL.Context;
using Core.Entities;
using Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        private readonly InsuranceDbContext _context;

        public VehicleRepository(InsuranceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetByClientIdAsync(int clientId)
        {
            return await _context.Vehicles
                .Where(v => v.ClientId == clientId)
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public void Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
        }

        public void Remove(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}