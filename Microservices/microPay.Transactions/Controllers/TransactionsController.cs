using microPay.Transactions.Entities;
using microPay.Transactions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace micropay.Transactions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _transactionsService;
        public TransactionsController(ITransactionsService transactionsService)
        {
            this._transactionsService = transactionsService;
        }

        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction(CreateTransactionRequest createRequest)
        {
            bool success = false;

            if (string.IsNullOrEmpty(createRequest.Username)
                || string.IsNullOrEmpty(createRequest.Action)
                || createRequest.Amount <= 0.0)
            {
                return new ObjectResult(success) { StatusCode = StatusCodes.Status400BadRequest };
            }

            try
            {
                success = await _transactionsService.CreateTransaction(createRequest);
            }
            catch (Exception)
            {
                return new ObjectResult(success) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }

            if (success)
            {
                return new ObjectResult(success) { StatusCode = StatusCodes.Status201Created };
            }
            else
            {
                return new ObjectResult(success) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }
        }

        [HttpGet("GetLatestTransactionsByUsername")]
        public async Task<IActionResult> GetLatestTransactionsByUsername(string username)
        {
            List<TransactionDTO> result = new List<TransactionDTO>();

            if (string.IsNullOrEmpty(username))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            try
            {
                result = await _transactionsService.GetLatestTransactionsByUsername(username);
            }
            catch (Exception ex)
            {
                result.Add(new TransactionDTO() { Username = ex.Message });
                return new ObjectResult(result) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }

            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
