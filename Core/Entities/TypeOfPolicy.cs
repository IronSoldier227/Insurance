using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class TypeOfPolicy
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();
}
