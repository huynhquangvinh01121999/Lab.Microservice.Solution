using AccountService.Application.DTOs.UserRoles;
using AccountService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AccountService.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleServices _userRoleServices;

        public UserRoleController(IUserRoleServices userRoleServices)
        {
            this._userRoleServices = userRoleServices;
        }

        [HttpPost("AddUserToRoles")]
        public async Task<IActionResult> Post(AddUserToRolesRequest request)
        {
            return Ok(await _userRoleServices.AddUserToRolesAsync(request));
        }
    }
}
