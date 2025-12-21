using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class PaymentForClaim
{
    public int Id { get; set; }

    public int ClaimId { get; set; }

    public int AuthorizedBy { get; set; }

    public double Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public virtual Manager AuthorizedByNavigation { get; set; } = null!;

    public virtual InsuranceClaim Claim { get; set; } = null!;
}
