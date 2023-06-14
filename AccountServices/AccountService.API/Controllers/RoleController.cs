using AccountService.Application.DTOs.Roles;
using AccountService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountService.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleServices _roleServices;

        public RoleController(IRoleServices roleServices)
        {
            _roleServices = roleServices;
        }

        [HttpPost("AddRoles")]
        public async Task<IActionResult> Post(AddRoleRequest request)
        {
            return Ok(await _roleServices.AddRoleAsync(request));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, string name)
        {
            return Ok(await _roleServices.UpdateRoleAsync(new UpdateRoleRequest { Id = id , Name = name}));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _roleServices.DeleteRoleAsync(id));
        }
    }
}
