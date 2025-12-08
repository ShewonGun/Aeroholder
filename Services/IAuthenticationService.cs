namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service interface for Authentication operations
    /// </summary>
    public interface IAuthenticationService
    {
        bool AuthenticateUser(string username, string password);
        bool IsUserActive(string username);
        bool ValidateCredentials(string username, string password);
    }
}
