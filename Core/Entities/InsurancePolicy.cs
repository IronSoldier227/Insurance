using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class InsurancePolicy
{
    public int Id { get; set; }

    public int VehicleId { get; set; }

    public int TypeId { get; set; }

    public int StatusId { get; set; }

    public int? CancelledBy { get; set; }

    public string PolicyNumber { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public double BasePrice { get; set; }

    public double PowerCoefficient { get; set; }

    public double ExperienceCoefficient { get; set; }

    public double BonusMalusCoefficient { get; set; }

    public double TotalPrice { get; set; }

    public virtual Manager? CancelledByNavigation { get; set; }

    public virtual ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();

    public virtual ICollection<PaymentForPolicy> PaymentForPolicies { get; set; } = new List<PaymentForPolicy>();

    public virtual StatusOfPolicy Status { get; set; } = null!;

    public virtual TypeOfPolicy Type { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
