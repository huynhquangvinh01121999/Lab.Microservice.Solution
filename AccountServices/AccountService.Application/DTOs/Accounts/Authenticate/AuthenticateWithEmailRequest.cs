namespace AccountService.Application.DTOs.Accounts.Authenticate
{
    public class AuthenticateWithEmailRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
