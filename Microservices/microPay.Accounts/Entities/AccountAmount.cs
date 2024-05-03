using System;
using System.Collections.Generic;

namespace microPay.Accounts.Entities;

public partial class AccountAmount
{
    public string Username { get; set; } = null!;

    public double Amount { get; set; }
}
