using microPay.Accounts.Controllers;
using microPay.Accounts.Entities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Net;

namespace microPay.Accounts.Tests
{
    [TestFixture]
    public class AccountsController_CreateAccount
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public async Task CreateAccount_Should_Return_201_On_Success()
        {
            //Arrange
            AccountDTO accountToCreate = new AccountDTO()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountToCreate))
                .ReturnAsync(accountToCreate);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.CreateAccount(accountToCreate);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.Created, "Account created");
        }

        [Test]
        public async Task CreateAccount_Should_Return_422_On_Creation_Fail()
        {
            //Arrange
            AccountDTO accountToCreate = new AccountDTO()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountToCreate))
                .ReturnAsync(null);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.CreateAccount(accountToCreate);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.UnprocessableContent, "Account create fail results in code 422");
        }


        [Test]
        public async Task CreateAccount_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            AccountDTO accountToCreate = new AccountDTO()
            {
                Username = "",
                Password = "password",
                Balance = null,
                CanOverdraft = "String"
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountToCreate))
                .ReturnAsync(null);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.CreateAccount(accountToCreate);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.BadRequest, "Account create invalid input results in code 400");
        }

        public async Task CreateAccount_Should_Return_409_When_Account_Already_Exists()
        {
            //Arrange
            AccountDTO accountToCreate = new AccountDTO()
            {
                Username = "EXISTS",
                Password = "password",
                Balance = 0.0,
                CanOverdraft = 0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountToCreate))
                .Throws(new AccountAlreadyExistsException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.CreateAccount(accountToCreate);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.Conflict, "Account create user exists results in code 409");
        }
    }

    [TestFixture]
    public class AccountsController_GetBalanceByUsername
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public async Task GetBalanceByUsername_On_Success()
        {
            //Arrange
            string accountToGet = "USER";
            AccountDTO expectedResult = new AccountDTO()
            {
                Username = "USER",
                Password = "password",
                Balance = 3.0,
                CanOverdraft = 1
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .ReturnAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.GetBalanceByUsername(accountToGet);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.OK, "Account retrieved");
            Assert.That(result.Value != null);
            Assert.That(result.Value.Username == "USER");
            Assert.That(result.Value.Balance == 3.0);

        }


        [Test]
        public async Task GetBalanceByUsername_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            string accountToGet = "";
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .ReturnAsync(null);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.GetBalanceByUsername(accountToGet);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.BadRequest, "Invalid request");
        }

        public async Task GetBalanceByUsername_Message_If_User_Does_Not_Exist()
        {
            //Arrange
            string accountToGet = "NOTEXIST";
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .ReturnAsync(null);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.GetBalanceByUsername(accountToGet);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.OK, "Account retrieved");
            Assert.That(result.Value.Username == "User does not exist");
        }
    }

    [TestFixture]
    public class AccountsController_Deposit
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public async Task Deposit_True_On_Success()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "USER",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Deposit(accChangeRequest))
                .ReturnAsync(true);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Deposit(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.OK, "Deposit successful");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "Success");
        }


        [Test]
        public async Task Deposit_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "",
                Amount = "NAN"
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Deposit(accChangeRequest))
                .ReturnAsync(false);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Deposit(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.BadRequest, "Deposit failed - Error in request");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "Invalid Input");
        }

        public async Task Deposit_False_On_Fail()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "USER",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Deposit(accChangeRequest))
                .ReturnAsync(false);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Deposit(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.UnprocessableContent, "Deposit fail results in code 422");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "An error occurred during withdraw");
        }
    }


    [TestFixture]
    public class AccountsController_Withdraw
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public async Task Withdraw_Message_On_Success()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "USER",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Withdraw(accChangeRequest))
                .ReturnAsync(true);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Withdraw(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.OK, "Withdraw successful");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "Success");
        }


        [Test]
        public async Task Withdraw_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "",
                Amount = "NAN"
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Withdraw(accChangeRequest))
                .ReturnAsync(false);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Withdraw(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.BadRequest, "Withdraw failed - Error in request");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "Invalid Input");
        }

        public async Task Withdraw_Message_On_Fail()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "USER",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Withdraw(accChangeRequest))
                .ReturnAsync(false);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Withdraw(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.UnprocessableContent, "Withdraw fail results in code 422");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "An error occurred during withdraw");
        }

        public async Task Withdraw_Message_And_400_On_Overdraft_Error()
        {
            //Arrange
            AccChangeRequest accChangeRequest = new AccChangeRequest()
            {
                Username = "USER",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.Withdraw(accChangeRequest))
                .Throws(new AccountCannotOverdraftException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var result = await _controller.Withdraw(accChangeRequest);

            //Assert
            Assert.That(result != null, "Create response is not null");
            Assert.That(result.StatusCode == HttpStatusCode.BadRequest, "Withdraw fail due to overdraft results in 400");
            Assert.That(result.Value != null);
            Assert.That(result.Value == "The Account is not allowed to overdraft");
        }
    }
}