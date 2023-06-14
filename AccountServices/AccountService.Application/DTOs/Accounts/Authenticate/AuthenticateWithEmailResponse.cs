namespace AccountService.Application.DTOs.Accounts.Authenticate
{
    public class AuthenticateWithEmailResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string token { get; set; }
    }
}
