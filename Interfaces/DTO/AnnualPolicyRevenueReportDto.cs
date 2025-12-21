// Interfaces/DTO/AnnualPolicyRevenueReportDto.cs
using System;

namespace Interfaces.DTO
{
    public class AnnualPolicyRevenueReportDto
    {
        public int Year { get; set; }
        public int TotalPoliciesCount { get; set; }
        public double TotalRevenue { get; set; } // Общий доход
    }
}