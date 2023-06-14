using AccountService.Application.DTOs.Accounts.Authenticate;
using AccountService.Application.DTOs.Accounts.Register;
using AccountService.Application.Wrappers;
using System.Threading.Tasks;

namespace AccountService.Application.Interfaces
{
    public interface IAccountServices
    {
        Task<Response<AuthenticateWithEmailResponse>> AuthenticateWithEmail(string email, string password);
        Task<Response<string>> RegisterAsync(RegisterRequest request);
    }
}
