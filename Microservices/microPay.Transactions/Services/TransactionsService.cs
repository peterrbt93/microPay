using microPay.Transactions.Entities;
using microPay.Transactions.Services;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace microPay.Transactions.Services
{
    public class TransactionsService : ITransactionsService
    {
        public TransactionsContext transactionsContext;

        public TransactionsService(TransactionsContext transactionsContext)
        {
            this.transactionsContext = transactionsContext;
        }
        public async Task<bool> CreateTransaction(CreateTransactionRequest request)
        { 
            var entity = new Transaction()
            {
                Username = request.Username,
                Action = request.Action,
                Amount = request.Amount,
                CreatedDate = DateTime.UtcNow,
                NewBalance = request.NewBalance
            };

            await transactionsContext.Transactions.AddAsync(entity);
            await transactionsContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<TransactionDTO>> GetLatestTransactionsByUsername(string username)
        {
            var entities = await transactionsContext.Transactions
            .Where(t => t.Username == username)
            .OrderByDescending(t => t.CreatedDate)
            .Take(10)
            .Select(t => new TransactionDTO
                {
                    Username = t.Username,
                    Action = t.Action,
                    Amount = t.Amount,
                    CreatedDate = t.CreatedDate,
                    NewBalance = t.NewBalance
                })
            .ToListAsync();

            return entities;
        }
    }
}
