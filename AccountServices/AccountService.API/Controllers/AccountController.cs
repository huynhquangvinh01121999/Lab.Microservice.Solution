using AccountService.Application.DTOs.Accounts.Authenticate;
using AccountService.Application.DTOs.Accounts.Register;
using AccountService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccountService.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;

        public AccountController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Post(AuthenticateWithEmailRequest request)
        {
            return Ok(await _accountServices.AuthenticateWithEmail(request.Email, request.Password));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post(RegisterRequest request)
        {
            return Ok(await _accountServices.RegisterAsync(request));
        }
    }
}
