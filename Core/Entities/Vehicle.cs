using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class Vehicle
{
    public int Id { get; set; }

    public int ModelId { get; set; }

    public int ClientId { get; set; }

    public string Color { get; set; } = null!;

    public int YearOfProduction { get; set; }

    public string Vin { get; set; } = null!;

    public string PlateNum { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int PowerHp { get; set; }

    public virtual ClientProfile Client { get; set; } = null!;

    public virtual ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();

    public virtual Model Model { get; set; } = null!;
}
