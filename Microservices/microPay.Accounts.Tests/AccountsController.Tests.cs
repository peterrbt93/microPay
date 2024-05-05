using Google.Protobuf.WellKnownTypes;
using microPay.Accounts.Controllers;
using microPay.Accounts.Entities;
using microPay.Accounts.Services;
using Microsoft.AspNetCore.Mvc;
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
            Account accountToCreate = new Account()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            AccountDTO accountDTOToCreate = new AccountDTO(accountToCreate);

            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountDTOToCreate))
                .ReturnsAsync(true);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.CreateAccount(accountDTOToCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateAccount(accountDTOToCreate), Times.Once);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 201, "Account created status 201");
            Assert.That((bool?)(result?.Value), Is.EqualTo(true), "Account created");
        }

        [Test]
        public async Task CreateAccount_Should_Return_422_On_Creation_Fail()
        {
            //Arrange
            Account accountToCreate = new Account()
            {
                Username = "UNITTEST",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            AccountDTO accountDTOToCreate = new AccountDTO(accountToCreate);
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountDTOToCreate))
                .ReturnsAsync(false);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.CreateAccount(accountDTOToCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateAccount(accountDTOToCreate), Times.Once);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 422, "Account create fail results in code 422");
            Assert.That((bool?)(result?.Value), Is.EqualTo(false), "Account create failed");
        }


        [Test]
        public async Task CreateAccount_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            Account accountToCreate = new Account()
            {
                Username = "",
                Password = "password",
                Balance = 2.0,
                CanOverdraft = 1
            };
            AccountDTO accountDTOToCreate = new AccountDTO(accountToCreate);
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountDTOToCreate))
                .ReturnsAsync(false);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.CreateAccount(accountDTOToCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateAccount(accountDTOToCreate), Times.Never);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 400, "Account create invalid input results in code 400");
            Assert.That((bool?)(result?.Value), Is.EqualTo(false), "Account create failed");
        }

        [Test]
        public async Task CreateAccount_Should_Return_409_When_Account_Already_Exists()
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
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.CreateAccount(accountDTOToCreate))
                .Throws(new AccountAlreadyExistsException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.CreateAccount(accountDTOToCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateAccount(accountDTOToCreate), Times.Once);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 409, "Account create user exists results in code 409");
            Assert.That((bool?)(result?.Value), Is.EqualTo(false), "Account create failed");
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
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = accountToGet,
                Amount = 2.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .ReturnsAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.GetBalanceByUsername(accountToGet);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.GetBalanceByUsername(accountToGet), Times.Once);
            Assert.That(result != null, "Balance response is not null");
            Assert.That(result?.StatusCode == 200, "Balance retrieved");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo(accountToGet), "Balance value correct");
        }


        [Test]
        public async Task GetBalanceByUsername_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            string accountToGet = "";
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = accountToGet,
                Amount = 2.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .ReturnsAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.GetBalanceByUsername(accountToGet);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.GetBalanceByUsername(accountToGet), Times.Never);
            Assert.That(result != null, "Balance response is not null");
            Assert.That(result?.StatusCode == 400, "Invalid request");
        }

        [Test]
        public async Task GetBalanceByUsername_Message_If_User_Does_Not_Exist()
        {
            //Arrange
            string accountToGet = "NOTEXISTUSER";
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "NOTEXIST",
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .Throws(new AccountNotExistsException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.GetBalanceByUsername(accountToGet);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.GetBalanceByUsername(accountToGet), Times.Once);
            Assert.That(result != null, "Balance response is not null");
            Assert.That(result?.StatusCode == 200, "Account retrieved");
            Assert.That(value?.Username == "NOTEXIST");
        }

        [Test]
        public async Task GetBalanceByUsername_Message_If_Error_Occurs()
        {
            //Arrange
            string accountToGet = "ERROR";
            string errorMessage = "An Error Has Occured";
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = errorMessage,
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.GetBalanceByUsername(accountToGet))
                .Throws(new Exception(errorMessage));
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.GetBalanceByUsername(accountToGet);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.GetBalanceByUsername(accountToGet), Times.Once);
            Assert.That(result != null, "Balance response is not null");
            Assert.That(result?.StatusCode == 422, "Account get error");
            Assert.That(value?.Username == errorMessage);
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
        public async Task Deposit_On_Success()
        {
            //Arrange
            var accountToGet = "USER";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToGet,
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "Success",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"))
                .ReturnsAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Deposit(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"), Times.Once);
            Assert.That(result != null, "Deposit response is not null");
            Assert.That(result?.StatusCode == 200, "Deposit successful");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("Success"), "Deposit correct");
            Assert.That(value?.Amount, Is.EqualTo(5.0), "Deposit correct");
        }

        [Test]
        public async Task Deposit_On_User_Not_Exist()
        {
            //Arrange
            var accountToGet = "USER";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToGet,
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"))
                .Throws(new AccountNotExistsException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Deposit(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"), Times.Once);
            Assert.That(result != null, "Deposit response is not null");
            Assert.That(result?.StatusCode == 200, "Deposit successful");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("NOTEXIST"), "Deposit correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Deposit correct");
        }

        [Test]
        public async Task Deposit_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = "",
                Amount = 0.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "",
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"))
                .ReturnsAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Deposit(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"), Times.Never);
            Assert.That(result != null, "Deposit response is not null");
            Assert.That(result?.StatusCode == 400, "Deposit failed - Error in request");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("Deposit failed - Error in request"), "Deposit correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Deposit correct");
        }

        [Test]
        public async Task Deposit_Message_On_Fail()
        {
            //Arrange
            String errorMessage = "An Error Occurred";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = "USER",
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "USER",
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"))
                .Throws(new Exception(errorMessage));
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Deposit(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "DEPOSIT"), Times.Once);
            Assert.That(result != null, "Deposit response is not null");
            Assert.That(result?.StatusCode == 422, "Deposit fail results in code 422");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo(errorMessage), "Deposit correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Deposit correct");
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
            var accountToGet = "USER";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToGet,
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "Success",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"))
                .ReturnsAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Withdraw(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Times.Once);
            Assert.That(result != null, "Withdraw response is not null");
            Assert.That(result?.StatusCode == 200, "Withdraw successful");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("Success"), "Withdraw correct");
            Assert.That(value?.Amount, Is.EqualTo(5.0), "Withdraw correct");
        }


        [Test]
        public async Task Withdraw_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = "",
                Amount = 0.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "",
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"))
                .ReturnsAsync(expectedResult);
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Withdraw(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Times.Never);
            Assert.That(result != null, "Withdraw response is not null");
            Assert.That(result?.StatusCode == 400, "Withdraw failed - Error in request");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("Withdraw failed - Error in request"), "Withdraw correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Withdraw correct");
        }

        [Test]
        public async Task Withdraw_Message_On_Fail()
        {
            //Arrange
            string errorMessage = "An Error occurred";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = "USER",
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "USER",
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"))
                .Throws(new Exception(errorMessage));
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Withdraw(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Times.Once);
            Assert.That(result != null, "Withdraw response is not null");
            Assert.That(result?.StatusCode == 422, "Withdraw fail results in code 422");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo(errorMessage), "Withdraw error message correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Withdraw correct");
        }

        [Test]
        public async Task Withdraw_On_User_Not_Exist()
        {
            //Arrange
            var accountToGet = "USER";
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = accountToGet,
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "",
                Amount = 5.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"))
                .Throws(new AccountNotExistsException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Withdraw(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Times.Once);
            Assert.That(result != null, "Deposit response is not null");
            Assert.That(result?.StatusCode == 200, "Deposit successful");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("NOTEXIST"), "Deposit correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Deposit correct");
        }

        [Test]
        public async Task Withdraw_Message_And_400_On_Overdraft_Error()
        {
            //Arrange
            AccountAmount accChangeRequest = new AccountAmount()
            {
                Username = "USER",
                Amount = 5.0
            };
            AccountAmount expectedResult = new AccountAmount()
            {
                Username = "USER",
                Amount = 0.0
            };
            var mockService = new Mock<IAccountsService>();
            mockService
                .Setup(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"))
                .Throws(new AccountCannotOverdraftException());
            var _controller = new AccountsController(mockService.Object);

            //Act
            var response = await _controller.Withdraw(accChangeRequest);
            var result = response as ObjectResult;
            var value = result?.Value as AccountAmount;

            //Assert
            mockService.Verify(m => m.DepositOrWithdraw(accChangeRequest, "WITHDRAW"), Times.Once);
            Assert.That(result != null, "Withdraw response is not null");
            Assert.That(result?.StatusCode == 400, "Withdraw fail due to overdraft results in 400");
            Assert.That(value != null);
            Assert.That(value?.Username, Is.EqualTo("Withdraw failed - Account not allowed overdraft"), "Withdraw correct");
            Assert.That(value?.Amount, Is.EqualTo(0.0), "Withdraw correct");
        }
    }
}