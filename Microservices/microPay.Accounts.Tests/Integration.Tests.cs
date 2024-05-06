using Google.Protobuf.WellKnownTypes;
using microPay.Accounts.Controllers;
using microPay.Accounts.Entities;
using microPay.Accounts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

namespace microPay.Accounts.Tests
{
    [TestFixture]
    public class IntegrationTest
    {
        private AccountsContext _context;
        private AccountsService _service;
        private HttpClient _client;

        public void Dispose()
        {
            _context?.Dispose();
            _client?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AccountsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new AccountsContext(options);
            _service = new AccountsService(_context);
            _client = new HttpClient() { BaseAddress = new Uri("https://localhost:3000") };
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task AccountService_CanDeposit_Success()
        {
            var accountToChange = "TOCHANGEINT";
            double initAmount = 100.0;
            double addAmount = 50.0;

            Account accountToCreate = new Account()
            {
                Username = accountToChange,
                Password = "password",
                Balance = initAmount,
                CanOverdraft = 1,
                CreatedDate = DateTime.UtcNow
            };
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToChange,
                Amount = addAmount
            };

            //Act
            await _context.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();
            var change = await _service.DepositOrWithdraw(accChangeRequest, "DEPOSIT", true);
            var accountAfterChange = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountToChange);

            var requestUri = "/Transactions/GetLatestTransactionsByUsername?username="+ accountToChange;
            var response = await _client.GetAsync(requestUri);
            var responseContent = await response.Content.ReadAsStringAsync();
            List<TransactionDTO> result = JsonConvert.DeserializeObject<List<TransactionDTO>>(responseContent);



            Assert.That(change != null, "Deposit response is not null");
            Assert.That(accountAfterChange?.Username == accountToCreate.Username, "Deposit user is correct");
            Assert.That(accountAfterChange?.Balance, Is.EqualTo(initAmount + addAmount), "Deposit correct");
            Assert.That(result?.Last().NewBalance, Is.EqualTo(accountAfterChange?.Balance), "Deposit correct");
            Assert.That(result?.Last().Amount, Is.EqualTo(addAmount), "Deposit correct");
        }
    }

}