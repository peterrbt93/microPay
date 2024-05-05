using microPay.Transactions.Entities;
using microPay.Transactions.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Net;
using System.Security.Principal;
using static NUnit.Framework.Interfaces.TNode;

namespace microPay.Transactions.Tests
{
    [TestFixture]
    public class TransactionsService_CreateTransactions : IDisposable
    {
        private TransactionsContext _context;
        private TransactionsService _service;

        public void Dispose()
        {
            _context?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TransactionsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new TransactionsContext(options);
            _service = new TransactionsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task CreateTransaction_On_Success_Calls_Db_To_Create()
        {
            // Arrange
            CreateTransactionRequest toCreate = new CreateTransactionRequest()
            {
                Action = "DEPOSIT",
                Amount = 5.0,
                NewBalance = 100.0,
                Username = "USERDEPOSIT"
            };

            //Act
            bool result = await _service.CreateTransaction(toCreate);
            var created = await _context.Transactions.FirstOrDefaultAsync(a => a.Username == toCreate.Username);

            //Assert
            Assert.That(created != null, "Transaction created");
            Assert.That(result == true, "Result correct");
            Assert.That(created?.Username == toCreate.Username, "Values correct");
            Assert.That(created?.Action == toCreate.Action, "Values correct");
            Assert.That(created?.Amount == toCreate.Amount, "Values correct");
            Assert.That(created?.NewBalance == toCreate.NewBalance, "Values correct");
        }
    }

    [TestFixture]
    public class TransactionsService_GetLatestTransactionsByUsername
    {

        private TransactionsContext _context;
        private TransactionsService _service;

        public void Dispose()
        {
            _context?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TransactionsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new TransactionsContext(options);
            _service = new TransactionsService(_context);
        }

        public List<TransactionDTO> GetMockTransactionsDTO()
        {
            return new List<TransactionDTO>()
            {
                new TransactionDTO()
                {
                    Action = "DEPOSIT",
                    Username = "USER",
                    Amount = 50.0,
                    CreatedDate = DateTime.Parse("01/06/2024"),
                    NewBalance = 250.0
                 },
                new TransactionDTO()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
            };
        }

        public List<Transaction> GetMockTransactions()
        {
            return new List<Transaction>()
            {
                new Transaction()
                {
                    Action = "DEPOSIT",
                    Username = "USER",
                    Amount = 50.0,
                    CreatedDate = DateTime.Parse("01/06/2024"),
                    NewBalance = 250.0
                 },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                },
                new Transaction()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.Parse("01/05/2024"),
                    NewBalance = 200.0
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task GetLatestTransactionsByUsername_On_Success()
        {
            //Arrange
            string accountToGet = "USER";

            List<TransactionDTO> expectedResult = GetMockTransactionsDTO();
            List<Transaction> transactions = GetMockTransactions();

            //Act
            for (int i = 0; i < transactions.Count; i++)
            {
               await _context.Transactions.AddAsync(transactions[i]);
            }
            await _context.SaveChangesAsync();
            var result = await _service.GetLatestTransactionsByUsername(accountToGet);

            //Assert
            Assert.That(result != null, "Get Transactions response is not null");
            Assert.That(result?.Count == 10, "Get Transactions response has only 10 transactions");
            Assert.That(result?[0].Username, Is.EqualTo(accountToGet), "Transactions value correct");
            Assert.That(result?[0].Amount, Is.EqualTo(expectedResult[0].Amount), "Transactions value correct");
            Assert.That(result?[0].Action, Is.EqualTo(expectedResult[0].Action), "Transactions value correct");
            Assert.That(result?[0].NewBalance, Is.EqualTo(expectedResult[0].NewBalance), "Transactions value correct");
            Assert.That(result?[1].Username, Is.EqualTo(accountToGet), "Transactions value correct");
            Assert.That(result?[1].Amount, Is.EqualTo(expectedResult[1].Amount), "Transactions value correct");
            Assert.That(result?[1].Action, Is.EqualTo(expectedResult[1].Action), "Transactions value correct");
            Assert.That(result?[1].NewBalance, Is.EqualTo(expectedResult[1].NewBalance), "Transactions value correct");
        }
    }


}