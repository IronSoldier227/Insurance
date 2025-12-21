using Interfaces.Services;
using Interfaces.DTO;
using Interfaces.Repository;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BLL.Services
{
    public class ManagerService : IManagerService
    {
        private readonly Interfaces.Repository.IManagerRepository _managerRepository;
        private readonly Interfaces.Repository.IPolicyRepository _policyRepository;
        private readonly Interfaces.Repository.IClaimRepository _claimRepository;
        private readonly Interfaces.Repository.IRepository<Core.Entities.PaymentForClaim> _paymentRepo;
        private readonly DAL.Context.InsuranceDbContext _context;

        public ManagerService(Interfaces.Repository.IManagerRepository managerRepository,
            Interfaces.Repository.IPolicyRepository policyRepository,
            Interfaces.Repository.IClaimRepository claimRepository,
            Interfaces.Repository.IRepository<Core.Entities.PaymentForClaim> paymentRepo,
            DAL.Context.InsuranceDbContext context)
        {
            _managerRepository = managerRepository;
            _policyRepository = policyRepository;
            _claimRepository = claimRepository;
            _paymentRepo = paymentRepo;
            _context = context;
        }

        public Task<IEnumerable<Client>> SearchClientsAsync(string? name, string? phone, string? policyNumber)
        {
            return _managerRepository.SearchClientsAsync(name, phone, policyNumber);
        }

        public Task<ClientDetails?> GetClientDetailsAsync(int clientId)
        {
            return _managerRepository.GetClientDetailsAsync(clientId);
        }

        public async Task CancelPolicyAsync(int policyId, int managerId)
        {
            var p = await _policyRepository.GetByIdAsync(policyId);
            if (p == null) return;
            p.StatusId = 3; // cancelled
            p.CancelledBy = managerId;
            _policyRepository.Update(p);
        }

        public async Task DecideClaimAsync(int claimId, int managerId, double? payoutAmount)
        {
            var c = await _claimRepository.GetByIdAsync(claimId);
            if (c == null) return;
            if (payoutAmount.HasValue)
            {
                c.StatusId = 2; // approved
                c.ProcessedBy = managerId;
                _claimRepository.Update(c);

                // create payment
                var payment = new Core.Entities.PaymentForClaim
                {
                    ClaimId = c.Id,
                    AuthorizedBy = managerId,
                    Amount = payoutAmount.Value,
                    PaymentDate = System.DateTime.UtcNow
                };

                await _paymentRepo.AddAsync(payment);
                await _paymentRepo.SaveChangesAsync();
            }
            else
            {
                c.StatusId = 3; // rejected
                c.ProcessedBy = managerId;
                _claimRepository.Update(c);
            }
        }
    }
}
