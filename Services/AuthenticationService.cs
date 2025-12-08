using System;
using AeroHolder_new.Repositories;

namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service implementation for Authentication business logic
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public bool AuthenticateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                // Check if user exists and is active
                if (!_userRepository.UserExists(username))
                    return false;

                if (!_userRepository.IsActive(username))
                    return false;

                // Validate credentials
                return _userRepository.ValidateCredentials(username, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Authentication error", ex);
            }
        }

        public bool IsUserActive(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return _userRepository.IsActive(username);
        }

        public bool ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            return _userRepository.ValidateCredentials(username, password);
        }
    }
}
