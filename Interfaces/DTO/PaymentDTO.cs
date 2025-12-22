using System;

namespace Interfaces.DTO
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string AuthorizedByManagerName { get; set; } = "N/A"; 

        public DateTime ClaimDate { get; set; }
        public string PolicyNumber { get; set; } = "N/A";
    }
}