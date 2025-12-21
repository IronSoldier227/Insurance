using System;
using System.Collections.Generic;

namespace Core.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsClient { get; set; }

    public virtual ClientProfile? ClientProfile { get; set; }

    public virtual Manager? Manager { get; set; }
}
