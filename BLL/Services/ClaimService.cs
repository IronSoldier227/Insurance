using Interfaces.Services;
using Interfaces.DTO;
using Interfaces.Repository;
using Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class ClaimService : IClaimService
    {
        private readonly Interfaces.Repository.IClaimRepository _claimRepository;

        public ClaimService(Interfaces.Repository.IClaimRepository claimRepository)
        {
            _claimRepository = claimRepository;
        }



        public async Task<IEnumerable<Claim>> GetByClientIdAsync(int clientId)
        {
            var list = await _claimRepository.GetByClientIdAsync(clientId);
            return list.Select(c => new Claim
            {
                Id = c.Id,
                PolicyId = c.PolicyId,
                StatusId = c.StatusId,
                ProcessedBy = c.ProcessedBy,
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                Location = c.Location,
                EstimatedDamage = c.EstimatedDamage
            });
        }
        public async Task<int> CreateClaimAsync(Claim claimDto)
        {
            var entity = new InsuranceClaim
            {
                PolicyId = claimDto.PolicyId,
                StatusId = claimDto.StatusId,
                ClaimDate = claimDto.ClaimDate,
                Description = claimDto.Description,
                Location = claimDto.Location,
                EstimatedDamage = claimDto.EstimatedDamage
            };

            await _claimRepository.AddAsync(entity);
            return entity.Id;
        }

        public async Task<IEnumerable<Claim>> GetClientClaimsAsync(int clientId)
        {
            var list = await _claimRepository.GetByClientIdAsync(clientId);
            return list.Select(c => new Claim
            {
                Id = c.Id,
                PolicyId = c.PolicyId,
                StatusId = c.StatusId,
                ProcessedBy = c.ProcessedBy,
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                Location = c.Location,
                EstimatedDamage = c.EstimatedDamage
            });
        }

        public async Task<Claim?> GetClaimByIdAsync(int id)
        {
            var c = await _claimRepository.GetByIdAsync(id);
            if (c == null) return null;
            return new Claim
            {
                Id = c.Id,
                PolicyId = c.PolicyId,
                StatusId = c.StatusId,
                ProcessedBy = c.ProcessedBy,
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                Location = c.Location,
                EstimatedDamage = c.EstimatedDamage
            };
        }

        public async Task DecideClaimAsync(int claimId, int managerId, double? payoutAmount)
        {
            var c = await _claimRepository.GetByIdAsync(claimId);
            if (c == null) return;
            if (payoutAmount.HasValue)
            {
                c.StatusId = 2;
                c.ProcessedBy = managerId;
                _claimRepository.Update(c);
            }
            else
            {
                c.StatusId = 3;
                c.ProcessedBy = managerId;
                _claimRepository.Update(c);
            }   
        }

        public async Task<IEnumerable<Claim>> GetAllClaimsAsync()
        {
            var list = await _claimRepository.GetAllWithRelatedDataAsync();
            return list.Select(c => new Claim
            {
                Id = c.Id,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy.PolicyNumber,
                StatusId = c.StatusId,
                StatusName = GetStatusName(c.StatusId), 
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                Location = c.Location,
                EstimatedDamage = c.EstimatedDamage
            }).ToList();
        }

        private string GetStatusName(int statusId)
        {
            return statusId switch
            {
                1 => "Отправлен",
                2 => "Одобрено",
                3 => "Отклонено",
                _ => "Неизвестно"
            };
        }
    }
}
