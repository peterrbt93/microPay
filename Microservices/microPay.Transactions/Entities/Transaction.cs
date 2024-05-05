using System;
using System.Collections.Generic;

namespace microPay.Transactions.Entities;

public partial class Transaction
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Action { get; set; } = null!;

    public double Amount { get; set; }

    public DateTime CreatedDate { get; set; }

    public double NewBalance { get; set; }
}
