using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class InsuranceClaim
{
    public int Id { get; set; }

    public int PolicyId { get; set; }

    public int StatusId { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime ClaimDate { get; set; }

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public double EstimatedDamage { get; set; }

    public virtual ICollection<PaymentForClaim> PaymentForClaims { get; set; } = new List<PaymentForClaim>();

    public virtual InsurancePolicy Policy { get; set; } = null!;

    public virtual Manager? ProcessedByNavigation { get; set; }

    public virtual StatusOfClaim Status { get; set; } = null!;
}
