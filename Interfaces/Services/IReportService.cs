using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IReportService
    {
        Task<double> GetTotalPayoutsForYearAsync(int year);
        Task<AnnualPolicyRevenueReportDto> GetAnnualRevenueReportAsync(int year);
    }
}