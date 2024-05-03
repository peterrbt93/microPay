using microPay.Accounts.Entities;

namespace microPay.Accounts.Services
{
    public interface IAccountsService
    {
        public Task<AccountAmount> GetBalanceByUsername(string username);
        public Task<AccountDTO> GetAccountByUsername(string username);
        public Task<bool> CreateAccount(AccountDTO accountToCreate);
        public Task<AccountAmount> DepositOrWithdraw(AccountAmount accChangeRequest, string type);
    }
}
