using microPay.Transactions.Entities;
using Microsoft.AspNetCore.Mvc;

namespace microPay.Transactions.Services
{
    public interface ITransactionsService
    {
        public Task<List<TransactionDTO>> GetLatestTransactionsByUsername(string username);

        public Task<bool> CreateTransaction(CreateTransactionRequest createTransactionRequest);
    }
}
