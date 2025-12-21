// Interfaces/Services/IReportService.cs
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IReportService
    {
        Task<double> GetTotalPayoutsForYearAsync(int year);
    }
}