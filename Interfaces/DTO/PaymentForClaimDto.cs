// Interfaces/DTO/PaymentForClaimDto.cs
using System;

namespace Interfaces.DTO
{
    public class PaymentForClaimDto
    {
        public int ClaimId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int AuthorizedBy { get; set; } // Id менеджера
    }
}