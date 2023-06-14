using Microsoft.AspNetCore.Identity;

namespace AccountService.Infrastructure.Models.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
