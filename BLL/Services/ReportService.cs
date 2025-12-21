// BLL/Services/ReportService.cs
using Interfaces.Services;
using Interfaces.Repository;
using Core.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<PaymentForClaim> _paymentRepository;

        public ReportService(IRepository<PaymentForClaim> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<double> GetTotalPayoutsForYearAsync(int year)
        {
            var allPayments = await _paymentRepository.GetAllAsync();
            return allPayments
                .Where(p => p.PaymentDate.Year == year)
                .Sum(p => p.Amount);
        }
    }
}