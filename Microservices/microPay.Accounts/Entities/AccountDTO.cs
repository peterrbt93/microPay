using System;
using System.Collections.Generic;

namespace microPay.Accounts.Entities;

public class AccountDTO
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public double Balance { get; set; }

    public sbyte CanOverdraft { get; set; }
}
