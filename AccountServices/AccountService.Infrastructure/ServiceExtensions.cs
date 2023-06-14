using AccountService.Application.Interfaces;
using AccountService.Infrastructure.Contexts;
using AccountService.Infrastructure.Models.IdentityModels;
using AccountService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace AccountService.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddAccountInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region Add DbContext
            services.AddDbContext<AccountContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("AccountConnection"),
                    b => b.MigrationsAssembly(typeof(AccountContext).Assembly.FullName)));
            #endregion

            #region Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(
                config => {
                    config.Password.RequireDigit = true;
                    config.Password.RequiredUniqueChars = 0;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 3;
                    //config.SignIn.RequireConfirmedEmail = false;
                    //config.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<AccountContext>()
                .AddDefaultTokenProviders();
            #endregion

            #region Add Authenticate
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                });
            #endregion

            #region Add Trasient/Scope/Singleton
            services.AddTransient<IAccountServices, AccountServices>();
            services.AddTransient<IRoleServices, RoleServices>();
            services.AddTransient<IUserRoleServices, UserRoleServices>();
            #endregion
        }
    }
}
