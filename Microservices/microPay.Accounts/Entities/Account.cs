using System;
using System.Collections.Generic;

namespace microPay.Accounts.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public double Balance { get; set; }

    public DateTime CreatedDate { get; set; }

    public sbyte CanOverdraft { get; set; }
}
