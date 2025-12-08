using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AeroHolder_new.Data;
using AeroHolder_new.Models;

namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository implementation for Shareholder data access
    /// </summary>
    public class ShareholderRepository : IShareholderRepository
    {
        private readonly AppDbContext _context;

        public ShareholderRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<ShareholderModel> GetAll()
        {
            string query = @"
                SELECT 
                    s.ShareholderID, s.FirstName, s.LastName, s.FullName, s.FolioID, 
                    s.Country, s.Address1, s.Address2, s.City, s.CompanyIndividual, 
                    s.NoOfShares, s.NoOfTicketsIssued, s.Entitlement, s.CreatedDate, s.ModifiedDate,
                    (SELECT COUNT(*) FROM Passports WHERE ShareholderID = s.ShareholderID) AS NoOfPassports
                FROM Shareholders s
                ORDER BY s.FolioID";

            DataTable dt = _context.ExecuteQuery(query);
            return MapDataTableToList(dt);
        }

        public ShareholderModel GetById(int id)
        {
            string query = @"
                SELECT 
                    s.ShareholderID, s.FirstName, s.LastName, s.FullName, s.FolioID, 
                    s.Country, s.Address1, s.Address2, s.City, s.CompanyIndividual, 
                    s.NoOfShares, s.NoOfTicketsIssued, s.Entitlement, s.CreatedDate, s.ModifiedDate,
                    (SELECT COUNT(*) FROM Passports WHERE ShareholderID = s.ShareholderID) AS NoOfPassports
                FROM Shareholders s
                WHERE s.ShareholderID = @ShareholderID";

            SqlParameter[] parameters = { new SqlParameter("@ShareholderID", id) };
            DataTable dt = _context.ExecuteQuery(query, parameters);
            
            return dt.Rows.Count > 0 ? MapDataRowToModel(dt.Rows[0]) : null;
        }

        public ShareholderModel GetByFolioId(string folioId)
        {
            string query = @"
                SELECT 
                    s.ShareholderID, s.FirstName, s.LastName, s.FullName, s.FolioID, 
                    s.Country, s.Address1, s.Address2, s.City, s.CompanyIndividual, 
                    s.NoOfShares, s.NoOfTicketsIssued, s.Entitlement, s.CreatedDate, s.ModifiedDate,
                    (SELECT COUNT(*) FROM Passports WHERE ShareholderID = s.ShareholderID) AS NoOfPassports
                FROM Shareholders s
                WHERE s.FolioID = @FolioID";

            SqlParameter[] parameters = { new SqlParameter("@FolioID", folioId) };
            DataTable dt = _context.ExecuteQuery(query, parameters);
            
            return dt.Rows.Count > 0 ? MapDataRowToModel(dt.Rows[0]) : null;
        }

        public List<ShareholderModel> Search(string searchTerm)
        {
            string query = @"
                SELECT 
                    s.ShareholderID, s.FirstName, s.LastName, s.FullName, s.FolioID, 
                    s.Country, s.Address1, s.Address2, s.City, s.CompanyIndividual, 
                    s.NoOfShares, s.NoOfTicketsIssued, s.Entitlement, s.CreatedDate, s.ModifiedDate,
                    (SELECT COUNT(*) FROM Passports WHERE ShareholderID = s.ShareholderID) AS NoOfPassports
                FROM Shareholders s
                WHERE s.FolioID LIKE @SearchTerm 
                   OR s.FirstName LIKE @SearchTerm 
                   OR s.LastName LIKE @SearchTerm 
                   OR s.FullName LIKE @SearchTerm
                ORDER BY s.FolioID";

            SqlParameter[] parameters = { new SqlParameter("@SearchTerm", "%" + searchTerm + "%") };
            DataTable dt = _context.ExecuteQuery(query, parameters);
            return MapDataTableToList(dt);
        }

        public bool Add(ShareholderModel shareholder)
        {
            string query = @"
                INSERT INTO Shareholders 
                (FirstName, LastName, FullName, FolioID, Country, Address1, Address2, City, 
                 CompanyIndividual, NoOfShares, NoOfTicketsIssued, Entitlement, CreatedDate, ModifiedDate) 
                VALUES 
                (@FirstName, @LastName, @FullName, @FolioID, @Country, @Address1, @Address2, @City, 
                 @CompanyIndividual, @NoOfShares, @NoOfTicketsIssued, @Entitlement, GETDATE(), GETDATE())";

            SqlParameter[] parameters = CreateParameters(shareholder);
            return _context.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Update(ShareholderModel shareholder)
        {
            string query = @"
                UPDATE Shareholders 
                SET FirstName = @FirstName, LastName = @LastName, FullName = @FullName, 
                    Country = @Country, Address1 = @Address1, Address2 = @Address2, City = @City, 
                    CompanyIndividual = @CompanyIndividual, NoOfShares = @NoOfShares, 
                    NoOfTicketsIssued = @NoOfTicketsIssued, Entitlement = @Entitlement,
                    ModifiedDate = GETDATE()
                WHERE FolioID = @FolioID";

            SqlParameter[] parameters = CreateParameters(shareholder);
            return _context.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Delete(string folioId)
        {
            string query = "DELETE FROM Shareholders WHERE FolioID = @FolioID";
            SqlParameter[] parameters = { new SqlParameter("@FolioID", folioId) };
            return _context.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool Exists(string folioId)
        {
            string query = "SELECT COUNT(*) FROM Shareholders WHERE FolioID = @FolioID";
            SqlParameter[] parameters = { new SqlParameter("@FolioID", folioId) };
            object result = _context.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        public int Count()
        {
            string query = "SELECT COUNT(*) FROM Shareholders";
            object result = _context.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        private ShareholderModel MapDataRowToModel(DataRow row)
        {
            return new ShareholderModel
            {
                ShareholderID = Convert.ToInt32(row["ShareholderID"]),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                FullName = row["FullName"].ToString(),
                FolioID = row["FolioID"].ToString(),
                Country = row["Country"]?.ToString(),
                Address1 = row["Address1"]?.ToString(),
                Address2 = row["Address2"]?.ToString(),
                City = row["City"]?.ToString(),
                CompanyIndividual = row["CompanyIndividual"]?.ToString(),
                NoOfShares = Convert.ToInt32(row["NoOfShares"]),
                NoOfTicketsIssued = Convert.ToInt32(row["NoOfTicketsIssued"]),
                Entitlement = Convert.ToInt32(row["Entitlement"]),
                NoOfPassports = row.Table.Columns.Contains("NoOfPassports") ? Convert.ToInt32(row["NoOfPassports"]) : 0,
                CreatedDate = row.Table.Columns.Contains("CreatedDate") ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now,
                ModifiedDate = row.Table.Columns.Contains("ModifiedDate") ? Convert.ToDateTime(row["ModifiedDate"]) : DateTime.Now
            };
        }

        private List<ShareholderModel> MapDataTableToList(DataTable dt)
        {
            List<ShareholderModel> shareholders = new List<ShareholderModel>();
            foreach (DataRow row in dt.Rows)
            {
                shareholders.Add(MapDataRowToModel(row));
            }
            return shareholders;
        }

        private SqlParameter[] CreateParameters(ShareholderModel shareholder)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@FirstName", shareholder.FirstName),
                new SqlParameter("@LastName", shareholder.LastName),
                new SqlParameter("@FullName", shareholder.FullName),
                new SqlParameter("@FolioID", shareholder.FolioID),
                new SqlParameter("@Country", shareholder.Country ?? (object)DBNull.Value),
                new SqlParameter("@Address1", shareholder.Address1 ?? (object)DBNull.Value),
                new SqlParameter("@Address2", shareholder.Address2 ?? (object)DBNull.Value),
                new SqlParameter("@City", shareholder.City ?? (object)DBNull.Value),
                new SqlParameter("@CompanyIndividual", shareholder.CompanyIndividual ?? (object)DBNull.Value),
                new SqlParameter("@NoOfShares", shareholder.NoOfShares),
                new SqlParameter("@NoOfTicketsIssued", shareholder.NoOfTicketsIssued),
                new SqlParameter("@Entitlement", shareholder.Entitlement)
            };
        }
    }
}
