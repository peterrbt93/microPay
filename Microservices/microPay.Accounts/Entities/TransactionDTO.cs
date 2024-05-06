using System;
using System.Collections.Generic;

namespace microPay.Accounts.Entities;

public class TransactionDTO
{
    public string Action { get; set; } = null!;

    public string Username { get; set; } = null!;

    public double Amount { get; set; }

    public DateTime CreatedDate { get; set; }

    public double NewBalance { get; set; }
}
