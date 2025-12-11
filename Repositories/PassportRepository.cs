using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AeroHolder_new.Data;
using AeroHolder_new.Models;

namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository for Passport data access
    /// </summary>
    public class PassportRepository
    {
        private readonly AppDbContext _context;

        public PassportRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Add a new passport to the database
        /// </summary>
        public bool Add(PassportModel passport)
        {
            try
            {
                string query = @"
                    INSERT INTO Passports 
                    (ShareholderID, PassportNumber, IssuedCountry, IssueDate, ExpiryDate) 
                    VALUES 
                    (@ShareholderID, @PassportNumber, @IssuedCountry, @IssueDate, @ExpiryDate)";

                SqlParameter[] parameters = {
                    new SqlParameter("@ShareholderID", passport.ShareholderID),
                    new SqlParameter("@PassportNumber", passport.PassportNumber ?? (object)DBNull.Value),
                    new SqlParameter("@IssuedCountry", passport.IssuedCountry ?? (object)DBNull.Value),
                    new SqlParameter("@IssueDate", passport.IssuedDate ?? (object)DBNull.Value),
                    new SqlParameter("@ExpiryDate", passport.ExpiryDate ?? (object)DBNull.Value)
                };

                return _context.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding passport: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all passports for a specific shareholder
        /// </summary>
        public List<PassportModel> GetByShareholderID(int shareholderID)
        {
            try
            {
                string query = @"
                    SELECT PassportID, ShareholderID, PassportNumber, IssuedCountry, 
                           IssueDate, ExpiryDate
                    FROM Passports
                    WHERE ShareholderID = @ShareholderID";

                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderID) };
                DataTable dt = _context.ExecuteQuery(query, parameters);

                List<PassportModel> passports = new List<PassportModel>();
                foreach (DataRow row in dt.Rows)
                {
                    passports.Add(MapDataRowToModel(row));
                }
                return passports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting passports: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete all passports for a specific shareholder
        /// </summary>
        public bool DeleteByShareholderID(int shareholderID)
        {
            try
            {
                string query = "DELETE FROM Passports WHERE ShareholderID = @ShareholderID";
                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderID) };
                return _context.ExecuteNonQuery(query, parameters) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting passports: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get count of passports for a shareholder
        /// </summary>
        public int GetCountByShareholderID(int shareholderID)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Passports WHERE ShareholderID = @ShareholderID";
                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderID) };
                object result = _context.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting passports: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map DataRow to PassportModel
        /// </summary>
        private PassportModel MapDataRowToModel(DataRow row)
        {
            return new PassportModel
            {
                PassportID = Convert.ToInt32(row["PassportID"]),
                ShareholderID = Convert.ToInt32(row["ShareholderID"]),
                PassportNumber = row["PassportNumber"]?.ToString(),
                IssuedCountry = row["IssuedCountry"]?.ToString(),
                ExpiryDate = row["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(row["ExpiryDate"]) : (DateTime?)null,
                IssuedDate = row["IssueDate"] != DBNull.Value ? Convert.ToDateTime(row["IssueDate"]) : (DateTime?)null,
                CreatedDate = DateTime.Now
            };
        }
    }
}
