using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class ClientProfile
{
    public int Id { get; set; }

    public string Passport { get; set; } = null!;

    public string DriverLicense { get; set; } = null!;

    public int DrivingExperience { get; set; }

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<PaymentForPolicy> PaymentForPolicies { get; set; } = new List<PaymentForPolicy>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
