using System;
using System.Data.SqlClient;
using AeroHolder_new.Data;

namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository implementation for User data access
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool ValidateCredentials(string username, string password)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE Username = @Username 
                  AND PasswordHash = @Password 
                  AND IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            object result = _context.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public bool UserExists(string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            SqlParameter[] parameters = { new SqlParameter("@Username", username) };
            object result = _context.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public bool IsActive(string username)
        {
            string query = "SELECT IsActive FROM Users WHERE Username = @Username";
            SqlParameter[] parameters = { new SqlParameter("@Username", username) };
            object result = _context.ExecuteScalar(query, parameters);
            return result != null && Convert.ToBoolean(result);
        }
    }
}
