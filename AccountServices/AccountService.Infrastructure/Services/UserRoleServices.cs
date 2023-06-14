using AccountService.Application.DTOs.UserRoles;
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
    public class UserRoleServices : IUserRoleServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AccountContext _context;
        private readonly IConfiguration _configuration;

        public UserRoleServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, AccountContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Response<IList<string>>> AddUserToRolesAsync(AddUserToRolesRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user is null)
                    return new Response<IList<string>>("User not exists");

                var listErrors = new StringBuilder();
                var listSuccess = new List<string>();
                foreach (var roleName in request.Roles)
                {
                    // get list roles of user
                    var getRolesForUser = await _userManager.GetRolesAsync(user);

                    // check role if exist
                    var getRole = getRolesForUser.Where(x => x.Equals(roleName)).FirstOrDefault();
                    if(getRole != null)
                    {
                        listErrors.Append(",").Append(roleName);
                        continue;
                    }

                    // check roleName if exist
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if(role == null)
                    {
                        listErrors.Append(",").Append(roleName);
                        continue;
                    }

                    // add user to role
                    var result = await _userManager.AddToRoleAsync(user, roleName);
                    if (!result.Succeeded)
                    {
                        listErrors.Append(",").Append(roleName);
                        continue;
                    }

                    listSuccess.Add(roleName);
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
    }
}
