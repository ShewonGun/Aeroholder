namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository interface for User data access
    /// </summary>
    public interface IUserRepository
    {
        bool ValidateCredentials(string username, string password);
        bool UserExists(string username);
        bool IsActive(string username);
    }
}
