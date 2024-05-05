using microPay.Accounts.Entities;
using microPay.Accounts.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Net;
using System.Security.Principal;
using System.Xml;
using static NUnit.Framework.Interfaces.TNode;

namespace microPay.Accounts.Tests
{
    [TestFixture]
    public class AccountsService_CreateAccount : IDisposable
    {
        private AccountsContext _context;
        private AccountsService _service;

        public void Dispose()
        {
            _context?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AccountsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new AccountsContext(options);
            _service = new AccountsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task CreateAccount_On_Success_Calls_Db_To_Create()
        {
            // Arrange
            Account accountToCreate = new Account()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            AccountDTO accountDTOToCreate = new AccountDTO(accountToCreate);

            //Act
            bool result = await _service.CreateAccount(accountDTOToCreate);
            var accountCreated = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountDTOToCreate.Username);

            //Assert
            Assert.That(accountCreated != null, "Account created");
            Assert.That(result == true, "Result correct");
            Assert.That(accountCreated?.Username == accountDTOToCreate.Username, "Values correct");
            Assert.That(accountCreated?.Balance == accountDTOToCreate.Balance, "Values correct");
        }


        [Test]
        public async Task CreateAccount_Should_Throw_Error_When_Account_Already_Exists()
        {
            //Arrange
            Account accountToCreate = new Account()
            {
                Username = "EXISTS",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            AccountDTO accountDTOToCreate = new AccountDTO(accountToCreate);

            //Act
            await _context.Accounts.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();

            //Assert
            Assert.That(async () => await _service.CreateAccount(accountDTOToCreate), Throws.Exception.TypeOf<AccountAlreadyExistsException>());
        }
    }

    [TestFixture]
    public class AccountsService_GetAccountByUsername
    {

        private AccountsContext _context;
        private AccountsService _service;

        public void Dispose()
        {
            _context?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AccountsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new AccountsContext(options);
            _service = new AccountsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task GetAccountByUsername_On_Success()
        {
            //Arrange
            Account accountToCreate = new Account()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1,
                CreatedDate = DateTime.UtcNow
            };

            string accountToGet = accountToCreate.Username;

            //Act
            await _context.Accounts.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();
            var result = await _service.GetAccountByUsername(accountToGet);

            //Assert
            Assert.That(result != null, "Get account response is not null");
            Assert.That(result?.Username, Is.EqualTo(accountToGet), "Account value correct");
            Assert.That(result?.Password, Is.EqualTo(accountToCreate.Password), "Account value correct");
            Assert.That(result?.Balance, Is.EqualTo(accountToCreate.Balance), "Account value correct");
            Assert.That(result?.CanOverdraft, Is.EqualTo(accountToCreate.CanOverdraft), "Account value correct");
        }


        [Test]
        public async Task GetAccountByUsername_Should_Throw_Error_When_None_Exists()
        {
            //Arrange
            string accountToGet = "NOTEXIST";

            //Act

            //Assert
            Assert.That(async () => await _service.GetAccountByUsername(accountToGet), Throws.Exception.TypeOf<AccountNotExistsException>());
        }

    }

    [TestFixture]
    public class AccountsService_GetBalanceByUsername
    {

        private AccountsContext _context;
        private AccountsService _service;

        public void Dispose()
        {
            _context?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AccountsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new AccountsContext(options);
            _service = new AccountsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task GetBalanceByUsername_On_Success()
        {
            //Arrange
            Account accountToCreate = new Account()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1,
                CreatedDate = DateTime.UtcNow
            };

            string accountToGet = accountToCreate.Username;

            //Act
            await _context.Accounts.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();
            var result = await _service.GetBalanceByUsername(accountToGet);

            //Assert
            Assert.That(result != null, "Get account response is not null");
            Assert.That(result?.Username, Is.EqualTo(accountToGet), "Account value correct");
            Assert.That(result?.Amount, Is.EqualTo(accountToCreate.Balance), "Account value correct");
        }

        [Test]
        public async Task GetBalanceByUsername_Should_Throw_Error_When_None_Exists()
        {
            //Arrange
            string accountToGet = "NOTEXIST";

            //Act

            //Assert
            Assert.That(async () => await _service.GetBalanceByUsername(accountToGet), Throws.Exception.TypeOf<AccountNotExistsException>());
        }

    }


    [TestFixture]
    public class AccountsService_DepositOrWithdraw
    {

        private AccountsContext _context;
        private AccountsService _service;

        public void Dispose()
        {
            _context?.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AccountsContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new AccountsContext(options);
            _service = new AccountsService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public async Task DepositOrWithdraw_On_Deposit_Success()
        {
            //Arrange
            var accountToChange = "TOCHANGE";

            Account accountToCreate = new Account()
            {
                Username = accountToChange,
                Password = "password",
                Balance = 100.0,
                CanOverdraft = 1,
                CreatedDate = DateTime.UtcNow
            };
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToChange,
                Amount = 50.0
            };
            
            //Act
            await _context.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();
            var change = await _service.DepositOrWithdraw(accChangeRequest, "DEPOSIT");
            var accountAfterChange = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountToChange);


            //Assert
            Assert.That(change != null, "Deposit response is not null");
            Assert.That(accountAfterChange?.Username == accountToCreate.Username, "Deposit user is correct");
            Assert.That(accountAfterChange?.Balance, Is.EqualTo(accountToCreate.Balance+change?.Amount), "Deposit correct");
        }

        [Test]
        public async Task DepositOrWithdraw_On_Withdraw_Success()
        {
            //Arrange
            var accountToChange = "TOCHANGE";

            Account accountToCreate = new Account()
            {
                Username = accountToChange,
                Password = "password",
                Balance = 100.0,
                CanOverdraft = 1,
                CreatedDate = DateTime.UtcNow
            };
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToChange,
                Amount = 50.0
            };

            //Act
            await _context.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();
            var change = await _service.DepositOrWithdraw(accChangeRequest, "WITHDRAW");
            var accountAfterChange = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountToChange);


            //Assert
            Assert.That(change != null, "Withdraw response is not null");
            Assert.That(accountAfterChange?.Username == accountToCreate.Username, "Withdraw user is correct");
            Assert.That(accountAfterChange?.Balance, Is.EqualTo(accountToCreate.Balance - change?.Amount), "Withdraw correct");
        }

        [Test]
        public async Task DepositOrWithdraw_On_User_Not_Exist()
        {
            //Arrange
            var accountToChange = "NOTEXIST";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToChange,
                Amount = 5.0
            };

            //Act

            //Assert
            Assert.That(async () => await _service.DepositOrWithdraw(accChangeRequest, "DEPOSIT"), Throws.Exception.TypeOf<AccountNotExistsException>());
            Assert.That(async () => await _service.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Throws.Exception.TypeOf<AccountNotExistsException>());
        }

        [Test]
        public async Task DepositOrWithdraw_On_Overdraft_Success()
        {
            //Arrange
            var accountToChange = "TOCHANGE";

            Account accountToCreate = new Account()
            {
                Username = accountToChange,
                Password = "password",
                Balance = 100.0,
                CanOverdraft = 1,
                CreatedDate = DateTime.UtcNow
            };
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToChange,
                Amount = 150.0
            };

            //Act
            await _context.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();
            var change = await _service.DepositOrWithdraw(accChangeRequest, "WITHDRAW");
            var accountAfterChange = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountToChange);

            //Assert
            Assert.That(change != null, "Withdraw response is not null");
            Assert.That(accountAfterChange?.Username == accountToCreate.Username, "Withdraw user is correct");
            Assert.That(accountAfterChange?.Balance, Is.EqualTo(accountToCreate.Balance - change?.Amount), "Withdraw correct");

        }

        [Test]
        public async Task DepositOrWithdraw_On_Overdraft_Error()
        {
            //Arrange
            var accountToChange = "TOCHANGE";

            Account accountToCreate = new Account()
            {
                Username = accountToChange,
                Password = "password",
                Balance = 100.0,
                CanOverdraft = 0,
                CreatedDate = DateTime.UtcNow
            };
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToChange,
                Amount = 150.0
            };

            //Act
            await _context.AddAsync(accountToCreate);
            await _context.SaveChangesAsync();

            //Assert
            Assert.That(async () => await _service.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Throws.Exception.TypeOf<AccountCannotOverdraftException>());
            var accountAfterChange = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == accountToChange);
            Assert.That(accountAfterChange?.Balance == accountToCreate.Balance, "Withdraw correct");

        }
    }
}