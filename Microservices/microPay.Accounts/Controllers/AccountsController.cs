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
            throw new NotImplementedException();
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(AccountDTO account)
        {

            throw new NotImplementedException();
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit(AccountAmount accChangeRequest)
        {
            throw new NotImplementedException();
        }

        [HttpPost("Withdraw")]
        public async Task<IActionResult> Withdraw(AccountAmount accChangeRequest)
        {
            throw new NotImplementedException();
        }
    }
}
