using micropay.Transactions.Controllers;
using microPay.Transactions.Entities;
using microPay.Transactions.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Cryptography.X509Certificates;

namespace microPay.Transactions.Tests
{
    [TestFixture]
    public class TransactionsController_CreateTransaction
    {

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public async Task CreateTransaction_Should_Return_201_On_Success()
        {
            //Arrange
            CreateTransactionRequest toCreate = new CreateTransactionRequest()
            {
                Action = "DEPOSIT",
                Amount = 5.0,
                NewBalance = 100.0,
                Username = "USER"
            };

            var mockService = new Mock<ITransactionsService>();
            mockService
                .Setup(m => m.CreateTransaction(toCreate))
                .ReturnsAsync(true);
            var _controller = new TransactionsController(mockService.Object);

            //Act
            var response = await _controller.CreateTransaction(toCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateTransaction(toCreate), Times.Once);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 201, "Transaction created status 201");
            Assert.That((bool?)(result?.Value), Is.EqualTo(true), "Transaction created");
        }

        [Test]
        public async Task CreateTransaction_Should_Return_422_On_Creation_Fail()
        {
            //Arrange
            CreateTransactionRequest toCreate = new CreateTransactionRequest()
            {
                Action = "DEPOSIT",
                Amount = 5.0,
                NewBalance = 100.0,
                Username = "USER"
            };

            var mockService = new Mock<ITransactionsService>();
            mockService
                .Setup(m => m.CreateTransaction(toCreate))
                .ReturnsAsync(false);
            var _controller = new TransactionsController(mockService.Object);

            //Act
            var response = await _controller.CreateTransaction(toCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateTransaction(toCreate), Times.Once);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 422, "Transaction create fail results in code 422");
            Assert.That((bool?)(result?.Value), Is.EqualTo(false), "Transaction create failed");
        }


        [Test]
        public async Task CreateTransaction_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            CreateTransactionRequest toCreate = new CreateTransactionRequest()
            {
                Action = "",
                Amount = 5.0,
                NewBalance = 100.0,
                Username = "USER"
            };

            var mockService = new Mock<ITransactionsService>();
            mockService
                .Setup(m => m.CreateTransaction(toCreate))
                .ReturnsAsync(false);
            var _controller = new TransactionsController(mockService.Object);

            //Act
            var response = await _controller.CreateTransaction(toCreate);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.CreateTransaction(toCreate), Times.Never);
            Assert.That(result != null, "Create response is not null");
            Assert.That(result?.StatusCode == 400, "Transaction create invalid input results in code 400");
            Assert.That((bool?)(result?.Value), Is.EqualTo(false), "Transaction create failed");
        }
    }

    [TestFixture]
    public class TransactionsController_GetLatestTransactionsByUsername
    {

        [SetUp]
        public void SetUp()
        {
            
        }

        public List<TransactionDTO> GetMockTransactionsDTO() { 
            return new List<TransactionDTO>()
            {
                new TransactionDTO()
                {
                    Action = "DEPOSIT",
                    Username = "USER",
                    Amount = 50.0,
                    CreatedDate = DateTime.UtcNow,
                    NewBalance = 250.0
                 },
                new TransactionDTO()
                {
                    Action = "WITHDRAW",
                    Username = "USER",
                    Amount = 150.0,
                    CreatedDate = DateTime.UtcNow,
                    NewBalance = 200.0
                },
            };
        }

        [Test]
        public async Task GetLatestTransactionsByUsername_200_On_Success()
        {
            //Arrange
            string accountToGet = "USER";
            List<TransactionDTO> expectedResult = GetMockTransactionsDTO();

            var mockService = new Mock<ITransactionsService>();
            mockService
                .Setup(m => m.GetLatestTransactionsByUsername(accountToGet))
                .ReturnsAsync(expectedResult);
            var _controller = new TransactionsController(mockService.Object);

            //Act
            var response = await _controller.GetLatestTransactionsByUsername(accountToGet);
            var result = response as ObjectResult;
            var value = result?.Value as List<TransactionDTO>;

            //Assert
            mockService.Verify(m => m.GetLatestTransactionsByUsername(accountToGet), Times.Once);
            Assert.That(result != null, "GetTransaction response is not null");
            Assert.That(result?.StatusCode == 200, "GetTransaction retrieved");
            Assert.That(value != null);
            Assert.That(value?.Count, Is.EqualTo(2), "GetTransaction value correct");
            Assert.That(value?[0].NewBalance, Is.EqualTo(expectedResult[0].NewBalance), "GetTransaction value correct");
            Assert.That(value?[1].Amount, Is.EqualTo(expectedResult[1].Amount), "GetTransaction value correct");
        }


        [Test]
        public async Task GetLatestTransactionsByUsername_Should_Return_400_On_Invalid_Input()
        {
            //Arrange
            string accountToGet = "";
            List<TransactionDTO> expectedResult = GetMockTransactionsDTO();

            var mockService = new Mock<ITransactionsService>();
            mockService
                .Setup(m => m.GetLatestTransactionsByUsername(accountToGet))
                .ReturnsAsync(expectedResult);
            var _controller = new TransactionsController(mockService.Object);

            //Act
            var response = await _controller.GetLatestTransactionsByUsername(accountToGet);
            var result = response as ObjectResult;

            //Assert
            mockService.Verify(m => m.GetLatestTransactionsByUsername(accountToGet), Times.Never);
            Assert.That(result != null, "GetTransaction response is not null");
            Assert.That(result?.StatusCode == 400, "Invalid request");
        }


        [Test]
        public async Task GetLatestTransactionsByUsername_422_Message_If_Error_Occurs()
        {
            //Arrange
            string accountToGet = "ERROR";
            string errorMessage = "An Error Has Occured";
            List<TransactionDTO> expectedResult = GetMockTransactionsDTO();

            var mockService = new Mock<ITransactionsService>();
            mockService
                .Setup(m => m.GetLatestTransactionsByUsername(accountToGet))
                .Throws(new Exception(errorMessage));
            var _controller = new TransactionsController(mockService.Object);

            //Act
            var response = await _controller.GetLatestTransactionsByUsername(accountToGet);
            var result = response as ObjectResult;
            var value = result?.Value as List<TransactionDTO>;

            //Assert
            mockService.Verify(m => m.GetLatestTransactionsByUsername(accountToGet), Times.Once);
            Assert.That(result != null, "GetTransaction response is not null");
            Assert.That(result?.StatusCode == 422, "Transaction get error");
            Assert.That(value?[0].Username, Is.EqualTo(errorMessage), "GetTransaction value correct");
        }
    }
}