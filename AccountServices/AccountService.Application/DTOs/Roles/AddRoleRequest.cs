using System.Collections.Generic;

namespace AccountService.Application.DTOs.Roles
{
    public class AddRoleRequest
    {
        public IList<string> Roles { get; set; }
    }
}
