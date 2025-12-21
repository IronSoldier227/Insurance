using DAL.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.Repository;
using Interfaces.DTO;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly InsuranceDbContext _context;

        public ManagerRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> SearchClientsAsync(string? name, string? phone, string? policyNumber)
        {
            var query = _context.ClientProfiles.Include(c => c.IdNavigation).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Split(' ');
                query = query.Where(cp => parts.Any(p => cp.IdNavigation.FirstName.Contains(p) || cp.IdNavigation.LastName.Contains(p) || cp.IdNavigation.MiddleName.Contains(p)));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(cp => cp.IdNavigation.PhoneNumber.Contains(phone));
            }

            if (!string.IsNullOrWhiteSpace(policyNumber))
            {
                query = query.Where(cp => cp.Vehicles.Any(v => v.InsurancePolicies.Any(p => p.PolicyNumber.Contains(policyNumber))));
            }

            var list = await query.ToListAsync();
            return list.Select(cp => new Client
            {
                Id = cp.Id,
                FirstName = cp.IdNavigation.FirstName,
                LastName = cp.IdNavigation.LastName,
                MiddleName = cp.IdNavigation.MiddleName,
                PhoneNumber = cp.IdNavigation.PhoneNumber,
                PassportNumber = cp.Passport,
                DriverLicense = cp.DriverLicense,
                DrivingExperience = cp.DrivingExperience
            });
        }

        public async Task<ClientDetails?> GetClientDetailsAsync(int clientId)
        {
            var cp = await _context.ClientProfiles
                .Include(c => c.IdNavigation)
                .Include(c => c.Vehicles).ThenInclude(v => v.Model).ThenInclude(m => m.Brand)
                .Include(c => c.Vehicles).ThenInclude(v => v.InsurancePolicies)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (cp == null) return null;

            var vehicles = cp.Vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                ModelId = v.ModelId,
                Brand = v.Model.Brand.Name,
                Model = v.Model.Name,
                ClientId = v.ClientId,
                DriverName = cp.IdNavigation.FirstName + " " + cp.IdNavigation.LastName,
                Color = v.Color,
                YearOfProduction = v.YearOfProduction,
                Vin = v.Vin,
                PlateNum = v.PlateNum,
                Category = v.Category,
                PowerHp = v.PowerHp
            });

            var policies = cp.Vehicles.SelectMany(v => v.InsurancePolicies).Select(p => new Insurance
            {
                Id = p.Id,
                VehicleId = p.VehicleId,
                TypeId = p.TypeId,
                StatusId = p.StatusId,
                CancelledBy = p.CancelledBy,
                PolicyNumber = p.PolicyNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                BasePrice = p.BasePrice,
                PowerCoefficient = p.PowerCoefficient,
                ExperienceCoefficient = p.ExperienceCoefficient,
                BonusMalusCoefficient = p.BonusMalusCoefficient
            });

            var claims = _context.InsuranceClaims
                .Include(c => c.Policy).ThenInclude(p => p.Vehicle)
                .Where(c => c.Policy.Vehicle.ClientId == clientId)
                .Select(c => new Claim
                {
                    Id = c.Id,
                    PolicyId = c.PolicyId,
                    StatusId = c.StatusId,
                    ProcessedBy = c.ProcessedBy,
                    ClaimDate = c.ClaimDate,
                    Description = c.Description,
                    Location = c.Location,
                    EstimatedDamage = c.EstimatedDamage
                }).ToList();

            return new ClientDetails
            {
                Client = new Client
                {
                    Id = cp.Id,
                    FirstName = cp.IdNavigation.FirstName,
                    LastName = cp.IdNavigation.LastName,
                    MiddleName = cp.IdNavigation.MiddleName,
                    PhoneNumber = cp.IdNavigation.PhoneNumber,
                    PassportNumber = cp.Passport,
                    DriverLicense = cp.DriverLicense,
                    DrivingExperience = cp.DrivingExperience
                },
                Vehicles = vehicles,
                Policies = policies,
                Claims = claims
            };
        }
    }
}
