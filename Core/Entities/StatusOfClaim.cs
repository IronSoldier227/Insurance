using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class StatusOfClaim
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();
}
