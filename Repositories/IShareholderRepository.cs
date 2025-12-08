using System.Collections.Generic;
using AeroHolder_new.Models;

namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository interface for Shareholder data access
    /// </summary>
    public interface IShareholderRepository
    {
        // Read Operations
        List<ShareholderModel> GetAll();
        ShareholderModel GetById(int id);
        ShareholderModel GetByFolioId(string folioId);
        List<ShareholderModel> Search(string searchTerm);
        
        // Write Operations
        bool Add(ShareholderModel shareholder);
        bool Update(ShareholderModel shareholder);
        bool Delete(string folioId);
        
        // Validation
        bool Exists(string folioId);
        int Count();
    }
}
