using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(PaymentForClaimDto paymentDto);
    }
}