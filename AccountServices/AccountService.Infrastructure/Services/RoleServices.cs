using AccountService.Application.DTOs.Roles;
using AccountService.Application.Interfaces;
using AccountService.Application.Wrappers;
using AccountService.Infrastructure.Contexts;
using AccountService.Infrastructure.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.Services
{
    public class RoleServices : IRoleServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AccountContext _context;
        private readonly IConfiguration _configuration;

        public RoleServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, AccountContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Response<IList<string>>> AddRoleAsync(AddRoleRequest request)
        {
            try
            {
                var listErrors = new StringBuilder();
                var listSuccess = new List<string>();
                foreach (var item in request.Roles)
                {
                    var role = await _roleManager.FindByNameAsync(item);
                    if (role != null)
                    {
                        listErrors.Append(",").Append(item.ToString());
                        continue;
                    }

                    var newRole = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = item,
                        NormalizedName = item.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };
                    var result = await _roleManager.CreateAsync(newRole);

                    if (!result.Succeeded)
                    {
                        listErrors.Append(",").Append(item.ToString());
                        continue;
                    }

                    listSuccess.Add(newRole.Id);
                }

                if (listErrors.Length > 0)
                    return new Response<IList<string>>(listSuccess, listErrors.ToString().Substring(1));

                return new Response<IList<string>>(listSuccess);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<string>> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role is null)
                    return new Response<string>("Not found");
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    return new Response<string>("Delete failed");
                return new Response<string>(role.Id, "Remove successed");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<string>> UpdateRoleAsync(UpdateRoleRequest request)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.Id);
                if (role is null)
                    return new Response<string>($"Not found");

                var roleNameExist = await _roleManager.FindByNameAsync(request.Name);
                if (roleNameExist != null && roleNameExist.Id != role.Id)
                    return new Response<string>($"Role name {request.Name} has been existed");

                role.Name = request.Name;

                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                    return new Response<string>("Update failed");
                return new Response<string>(role.Id, "Update successed");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
