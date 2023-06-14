using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Application.DTOs.UserRoles
{
    public class AddUserToRolesRequest
    {
        public string UserId { get; set; }
        public IList<string> Roles { get; set; }
    }
}
