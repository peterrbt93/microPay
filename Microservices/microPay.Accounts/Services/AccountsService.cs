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
            throw new NotImplementedException();
        }

        public async Task<AccountDTO> GetAccountByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountAmount> GetBalanceByUsername(string username)
        {
            throw new NotImplementedException();
        }
        public async Task<AccountAmount> DepositOrWithdraw(AccountAmount accChangeRequest, string type)
        {
            throw new NotImplementedException();
            //This method needs to call transactions microservice to create transactions table
        }
    }
}
