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
        }
    }
}