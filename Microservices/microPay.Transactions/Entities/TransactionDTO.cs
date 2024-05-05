using System;
using System.Collections.Generic;

namespace microPay.Transactions.Entities;

public class TransactionDTO
{
    public string Action { get; set; } = null!;

    public string Username { get; set; } = null!;

    public double Amount { get; set; }

    public DateTime CreatedDate { get; set; }

    public double NewBalance { get; set; }

    public TransactionDTO(Transaction t ) {
        Action = t.Action;
        Username = t.Username;
        Amount = t.Amount;
        CreatedDate = t.CreatedDate;
        NewBalance = t.NewBalance;
    }

    public TransactionDTO() {}
}
