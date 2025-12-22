using System;

namespace Interfaces.DTO
{
    public class ReportDto
    {
        public int Year { get; set; }
        public int TotalCount { get; set; }
        public double TotalAmount { get; set; }
        public double Average { get; set; }
        public IEnumerable<ClientReportItemDto> ClientData {  get; set; }
    }
}