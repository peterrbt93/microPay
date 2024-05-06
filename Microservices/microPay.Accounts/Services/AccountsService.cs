using microPay.Accounts.Entities;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using XSystem.Security.Cryptography;

namespace microPay.Accounts.Services
{
    public class AccountsService : IAccountsService
    {
        public AccountsContext accountsContext;
        private readonly HttpClient client;

        public AccountsService(AccountsContext accountsContext)
        {
            this.accountsContext = accountsContext; 
            this.client = new HttpClient() { BaseAddress = new Uri("https://localhost:3000")};
        }
        public async Task<bool> CreateAccount(AccountDTO accountToCreate)
        {
            var entity = await accountsContext.Accounts.FirstOrDefaultAsync(s => s.Username == accountToCreate.Username);

            if (entity != null)
            {
                throw new AccountAlreadyExistsException();
            }

            var provider = new SHA1CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(accountToCreate.Password);
            string pwd = Convert.ToBase64String(provider.ComputeHash(bytes));

            entity = new Account()
            {
                Username = accountToCreate.Username,
                Password = pwd,
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
        public async Task<AccountAmount> DepositOrWithdraw(AccountAmount accChangeRequest, string type, bool withExternalCall = true)
        {
            var entity = await accountsContext.Accounts.FirstOrDefaultAsync(s => s.Username == accChangeRequest.Username);

            if (entity == null)
            {
                throw new AccountNotExistsException();
            }

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

            if (withExternalCall)
            {
                CreateTransactionRequest data = new CreateTransactionRequest()
                {
                    Username = entity.Username,
                    Action = type,
                    Amount = accChangeRequest.Amount,
                    NewBalance = entity.Balance
                };

                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var requestUri = "/Transactions/CreateTransaction";

                try
                {
                    var response = await client.PostAsync(requestUri, content);
                    response.EnsureSuccessStatusCode();
                    var responseContent = await response.Content.ReadAsStringAsync();
                    bool success = JsonConvert.DeserializeObject<bool>(responseContent);
                    if (!success) { throw new Exception($"Error while calling TransactionsAPI - returned false"); }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"Error while calling TransactionsAPI: {ex.Message}");
                }
            }
            
            await accountsContext.SaveChangesAsync();

            return accChangeRequest;
        }
    }
}
