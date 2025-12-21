using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class PaymentForPolicy
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int PolicyId { get; set; }

    public double Amount { get; set; }

    public virtual ClientProfile Client { get; set; } = null!;

    public virtual InsurancePolicy Policy { get; set; } = null!;
}
