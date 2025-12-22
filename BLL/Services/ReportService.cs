using Core.Entities;
using DAL.Context;
using Interfaces.DTO;
using Interfaces.Repository;
using Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<PaymentForClaim> _paymentRepository;
        private readonly InsuranceDbContext _context;

        public ReportService(IRepository<PaymentForClaim> paymentRepository, InsuranceDbContext context)
        {
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task<double> GetTotalPayoutsForYearAsync(int year)
        {
            var allPayments = await _paymentRepository.GetAllAsync();
            return allPayments
                .Where(p => p.PaymentDate.Year == year)
                .Sum(p => p.Amount);
        }

        public async Task<AnnualPolicyRevenueReportDto> GetAnnualRevenueReportAsync(int year)
        {
            var report = await _context.InsurancePolicies
                .Where(p => p.StartDate.Year == year) 
                .GroupBy(p => 1) 
                .Select(g => new AnnualPolicyRevenueReportDto
                {
                    Year = year,
                    TotalPoliciesCount = g.Count(),
                    TotalRevenue = g.Sum(p => p.TotalPrice)
                })
                .FirstOrDefaultAsync();

            return report ?? new AnnualPolicyRevenueReportDto { Year = year, TotalPoliciesCount = 0, TotalRevenue = 0 };
        }
    }
}