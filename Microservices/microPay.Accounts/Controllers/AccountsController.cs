using microPay.Accounts.Entities;
using microPay.Accounts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Security.Principal;

namespace microPay.Accounts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountsService;
        public AccountsController(IAccountsService accountsService)
        {
            this._accountsService = accountsService;
        }

        [HttpGet("GetBalanceByUsername")]
        public async Task<IActionResult> GetBalanceByUsername(string username)
        {
            AccountAmount result = new AccountAmount();

            if (string.IsNullOrEmpty(username))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            try
            {
                result = await _accountsService.GetBalanceByUsername(username);
            }
            catch (AccountNotExistsException)
            {
                result.Username = "NOTEXIST";
                result.Amount = 0.0;
                return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
            }
            catch (Exception ex)
            {
                result.Username = ex.Message;
                result.Amount = 0.0;
                return new ObjectResult(result) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }

            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(AccountDTO account)
        {

            bool success = false;

            if (string.IsNullOrEmpty(account.Username) 
                || string.IsNullOrEmpty(account.Password))
            {
                return new ObjectResult(success) { StatusCode = StatusCodes.Status400BadRequest };
            }

            try
            {
               success = await _accountsService.CreateAccount(account);
            }
            catch (AccountAlreadyExistsException)
            {
                return new ObjectResult(success) { StatusCode = StatusCodes.Status409Conflict};
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

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit(AccountAmount accChangeRequest)
        {
            AccountAmount response = accChangeRequest;

            if (string.IsNullOrEmpty(accChangeRequest.Username) || accChangeRequest.Amount <= 0.0)
            {
                response.Username = "Deposit failed - Error in request";
                return new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
            }

            try
            {
                response = await _accountsService.DepositOrWithdraw(accChangeRequest, "DEPOSIT");
            }
            catch (AccountNotExistsException)
            {
                response.Username = "NOTEXIST";
                response.Amount = 0.0;
                return new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
            }
            catch (Exception ex)
            {
                response.Username = ex.Message;
                response.Amount = 0.0;
                return new ObjectResult(response) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }

            return new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw(AccountAmount accChangeRequest)
        {
            AccountAmount response = accChangeRequest;

            if (string.IsNullOrEmpty(accChangeRequest.Username) || accChangeRequest.Amount <= 0.0)
            {
                response.Username = "Withdraw failed - Error in request";
                return new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
            }

            try
            {
                response = await _accountsService.DepositOrWithdraw(accChangeRequest, "WITHDRAW");
            }
            catch (AccountNotExistsException)
            {
                response.Username = "NOTEXIST";
                response.Amount = 0.0;
                return new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
            }
            catch (AccountCannotOverdraftException)
            {
                response.Username = "Withdraw failed - Account not allowed overdraft";
                response.Amount = 0.0;
                return new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
            }
            catch (Exception ex)
            {
                response.Username = ex.Message;
                response.Amount = 0.0;
                return new ObjectResult(response) { StatusCode = StatusCodes.Status422UnprocessableEntity };
            }

            return new ObjectResult(response) { StatusCode = StatusCodes.Status200OK };
        }
    }
}
