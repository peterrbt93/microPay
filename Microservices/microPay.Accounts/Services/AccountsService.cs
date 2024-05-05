using microPay.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;

namespace microPay.Accounts.Services
{
    public class AccountsService : IAccountsService
    {
        public AccountsContext accountsContext;

        public AccountsService(AccountsContext accountsContext)
        {
            this.accountsContext = accountsContext;
        }
        public async Task<bool> CreateAccount(AccountDTO accountToCreate)
        {
            var entity = await accountsContext.Accounts.FirstOrDefaultAsync(s => s.Username == accountToCreate.Username);

            if (entity != null)
            {
                throw new AccountAlreadyExistsException();
            }

            entity = new Account()
            {
                Username = accountToCreate.Username,
                Password = accountToCreate.Password,
                Balance = accountToCreate.Balance,
                CreatedDate = DateTime.UtcNow,
                CanOverdraft = accountToCreate.CanOverdraft
            };
            await accountsContext.Accounts.AddAsync(entity);
            await accountsContext.SaveChangesAsync();
            return true;
        }

        public async Task<AccountDTO> GetAccountByUsername(string username)
        {
            var entity = await accountsContext.Accounts.FirstOrDefaultAsync(s => s.Username == username);

            if (entity == null)
            {
                throw new AccountNotExistsException();
            }

            return new AccountDTO(entity);
        }

        public async Task<AccountAmount> GetBalanceByUsername(string username)
        {
            var entity = await accountsContext.Accounts.FirstOrDefaultAsync(s => s.Username == username);

            if (entity == null)
            {
                throw new AccountNotExistsException();
            }

            return new AccountAmount() { Username = entity.Username, Amount = entity.Balance };
        }
        public async Task<AccountAmount> DepositOrWithdraw(AccountAmount accChangeRequest, string type)
        {
            var entity = await accountsContext.Accounts.FirstOrDefaultAsync(s => s.Username == accChangeRequest.Username);

            if (entity == null)
            {
                throw new AccountNotExistsException();
            }

            bool callToTransactionAPISuccess = false;
            try
            {
                //TODO: Call transactionsAPI to create record
                callToTransactionAPISuccess = true;
            } 
            catch (Exception ex)
            {
                throw new Exception("Error calling transactionAPI: "+ ex.Message);
            }

            if (callToTransactionAPISuccess)
            {
                if (type == "DEPOSIT")
                {
                    entity.Balance += accChangeRequest.Amount;
                } 
                else if (type == "WITHDRAW")
                {
                    if (entity.CanOverdraft == 0 && entity.Balance - accChangeRequest.Amount < 0.0)
                    {
                        throw new AccountCannotOverdraftException();
                    } 
                    else
                    {
                        entity.Balance -= accChangeRequest.Amount;
                    }
                }
                
                await accountsContext.SaveChangesAsync();
                return accChangeRequest;
            } 
            else
            {
                throw new Exception("Calling transactionAPI failed");
            }
        }
    }
}
