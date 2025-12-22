using Interfaces.Services;
using Interfaces.DTO;
using Interfaces.Repository;
using Core.Entities;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<PaymentForClaim> _paymentRepository;

        public PaymentService(IRepository<PaymentForClaim> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task CreatePaymentAsync(PaymentForClaimDto paymentDto)
        {
            var entity = new PaymentForClaim
            {
                ClaimId = paymentDto.ClaimId,
                Amount = paymentDto.Amount,
                PaymentDate = paymentDto.PaymentDate,
                AuthorizedBy = paymentDto.AuthorizedBy
            };

            await _paymentRepository.AddAsync(entity);
            await _paymentRepository.SaveChangesAsync();
        }
    }
}