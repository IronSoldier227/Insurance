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

        public async Task<ReportDto> GetTotalPayoutsForYearAsync(int year)
        {
            var allPayments = await _paymentRepository.GetAllAsync();
            var yearPayments = allPayments.Where(p => p.PaymentDate.Year == year);
            double amount = yearPayments.Sum(p => p.Amount);
            int count = yearPayments.Count();
            double avg = amount / count;

            var payouts = await _context.PaymentForClaims
                .Include(p => p.Claim)
                    .ThenInclude(c => c.Policy)
                        .ThenInclude(p => p.Vehicle)
                            .ThenInclude(v => v.Client)
                                .ThenInclude(cp => cp.IdNavigation)
                .Where(p => p.PaymentDate.Year == year)
                .Select(p => new ClientReportItemDto
                {
                    ClientFullName = p.Claim.Policy.Vehicle.Client.IdNavigation.FirstName + " " +
                                     p.Claim.Policy.Vehicle.Client.IdNavigation.LastName + " " +
                                     p.Claim.Policy.Vehicle.Client.IdNavigation.MiddleName,
                    Amount = p.Amount
                })
                .ToListAsync();

            return new ReportDto
            {
                Year = year,
                TotalAmount = amount,
                TotalCount = count,
                Average = avg,
                ClientData = payouts
            };

        }

        public async Task<ReportDto> GetAnnualRevenueReportAsync(int year)
        {
            var yearPolicies = await _context.InsurancePolicies
                .Include(p => p.Vehicle)
                    .ThenInclude(v => v.Client)
                        .ThenInclude(cp => cp.IdNavigation)
                .Where(p => p.StartDate.Year == year) 
                .ToListAsync();

            var totalAmount = yearPolicies.Sum(p => p.TotalPrice);
            var totalCount = yearPolicies.Count;
            var average = totalCount > 0 ? totalAmount / totalCount : 0;

            var clientData = yearPolicies.Select(p => new ClientReportItemDto
            {
                ClientFullName = p.Vehicle.Client.IdNavigation.FirstName + " " +
                                 p.Vehicle.Client.IdNavigation.LastName + " " +
                                 p.Vehicle.Client.IdNavigation.MiddleName,
                Amount = p.TotalPrice
            }).ToList();

            return new ReportDto
            {
                Year = year,
                TotalCount = totalCount,
                TotalAmount = totalAmount,
                Average = average,
                ClientData = clientData
            };
        }
    }
}