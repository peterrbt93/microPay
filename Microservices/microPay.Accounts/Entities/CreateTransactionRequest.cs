using System;
using System.Collections.Generic;

namespace microPay.Accounts.Entities;

public partial class CreateTransactionRequest
{
    public string Action { get; set; } = null!;

    public double Amount { get; set; }

    public double NewBalance { get; set; }

    public string Username { get; set; } = null!;
}
