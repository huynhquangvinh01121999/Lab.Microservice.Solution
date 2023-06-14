using AccountService.Application.DTOs.Roles;
using AccountService.Application.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Application.Interfaces
{
    public interface IRoleServices
    {
        Task<Response<IList<string>>> AddRoleAsync(AddRoleRequest request);
        Task<Response<string>> UpdateRoleAsync(UpdateRoleRequest request);
        Task<Response<string>> DeleteRoleAsync(string roleId);
    }
}
