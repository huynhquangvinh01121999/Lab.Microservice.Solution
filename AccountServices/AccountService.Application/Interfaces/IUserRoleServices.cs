using AccountService.Application.DTOs.UserRoles;
using AccountService.Application.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Application.Interfaces
{
    public interface IUserRoleServices
    {
        Task<Response<IList<string>>> AddUserToRolesAsync(AddUserToRolesRequest request);
    }
}
