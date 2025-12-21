using System;

namespace Interfaces.DTO
{
    public class Claim
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int StatusId { get; set; }
        public int? ProcessedBy { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public double EstimatedDamage { get; set; }
        public string PolicyNumber { get; set; } = null!;
        public string StatusName { get; set; } = null!;
    }
}
