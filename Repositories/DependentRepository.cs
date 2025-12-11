using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AeroHolder_new.Data;
using AeroHolder_new.Models;

namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository for Dependent data access
    /// </summary>
    public class DependentRepository
    {
        private readonly AppDbContext _context;

        public DependentRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Add a new dependent to the database
        /// </summary>
        public bool Add(DependentModel dependent)
        {
            try
            {
                string query = @"
                    INSERT INTO Dependents 
                    (ShareholderID, FullName, Relationship, PassportNumber) 
                    VALUES 
                    (@ShareholderID, @FullName, @Relationship, @PassportNumber)";

                SqlParameter[] parameters = {
                    new SqlParameter("@ShareholderID", dependent.ShareholderID),
                    new SqlParameter("@FullName", dependent.FullName ?? (object)DBNull.Value),
                    new SqlParameter("@Relationship", dependent.Relationship ?? (object)DBNull.Value),
                    new SqlParameter("@PassportNumber", dependent.PassportNumber ?? (object)DBNull.Value)
                };

                return _context.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding dependent: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all dependents for a specific shareholder
        /// </summary>
        public List<DependentModel> GetByShareholderID(int shareholderID)
        {
            try
            {
                string query = @"
                    SELECT DependentID, ShareholderID, FullName, Relationship, PassportNumber
                    FROM Dependents
                    WHERE ShareholderID = @ShareholderID";

                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderID) };
                DataTable dt = _context.ExecuteQuery(query, parameters);

                List<DependentModel> dependents = new List<DependentModel>();
                foreach (DataRow row in dt.Rows)
                {
                    dependents.Add(MapDataRowToModel(row));
                }
                return dependents;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting dependents: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete all dependents for a specific shareholder
        /// </summary>
        public bool DeleteByShareholderID(int shareholderID)
        {
            try
            {
                string query = "DELETE FROM Dependents WHERE ShareholderID = @ShareholderID";
                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderID) };
                return _context.ExecuteNonQuery(query, parameters) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting dependents: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get count of dependents for a shareholder
        /// </summary>
        public int GetCountByShareholderID(int shareholderID)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Dependents WHERE ShareholderID = @ShareholderID";
                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderID) };
                object result = _context.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting dependents: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Map DataRow to DependentModel
        /// </summary>
        private DependentModel MapDataRowToModel(DataRow row)
        {
            return new DependentModel
            {
                DependentID = Convert.ToInt32(row["DependentID"]),
                ShareholderID = Convert.ToInt32(row["ShareholderID"]),
                FullName = row["FullName"]?.ToString(),
                Relationship = row["Relationship"]?.ToString(),
                PassportNumber = row["PassportNumber"]?.ToString(),
                CreatedDate = DateTime.Now
            };
        }
    }
}
