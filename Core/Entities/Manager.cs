using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class Manager
{
    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();

    public virtual ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();

    public virtual ICollection<PaymentForClaim> PaymentForClaims { get; set; } = new List<PaymentForClaim>();
}
