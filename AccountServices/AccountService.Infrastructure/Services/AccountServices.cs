using AccountService.Application.DTOs.Accounts.Authenticate;
using AccountService.Application.DTOs.Accounts.Register;
using AccountService.Application.Interfaces;
using AccountService.Application.Wrappers;
using AccountService.Infrastructure.Contexts;
using AccountService.Infrastructure.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Infrastructure.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AccountContext _context;
        private readonly IConfiguration _configuration;

        public AccountServices(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AccountContext context, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<Response<AuthenticateWithEmailResponse>> AuthenticateWithEmail(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is null)
                    return new Response<AuthenticateWithEmailResponse>($"Email {email} is not register.");

                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (!result.Succeeded)
                    return new Response<AuthenticateWithEmailResponse>("Password not correct.");

                var jwtSecurityToken = await GenerateJWToken(user);
                return new Response<AuthenticateWithEmailResponse>(new AuthenticateWithEmailResponse
                {
                    //token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
                    token = jwtSecurityToken.EncodedPayload,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userWithEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userWithEmail != null)
                    return new Response<string>($"Email {request.Email} already exists.");

                var newUser = new ApplicationUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(newUser, request.Password);
                if (!result.Succeeded)
                    return new Response<string>($"{result.Errors.FirstOrDefault().Description}");

                // Add role
                // await _userManager.AddToRoleAsync(newUser, "USER");

                return new Response<string>(newUser.Id, "Register successed.");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Double.Parse(_configuration["JWTSettings:DurationInMinutes"])),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}
