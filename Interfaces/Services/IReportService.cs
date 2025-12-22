using Interfaces.DTO;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IReportService
    {
        Task<ReportDto> GetTotalPayoutsForYearAsync(int year);
        Task<ReportDto> GetAnnualRevenueReportAsync(int year);
    }
}