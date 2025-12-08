using System.Collections.Generic;
using AeroHolder_new.Models;

namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service interface for Shareholder business operations
    /// </summary>
    public interface IShareholderService
    {
        // Query Operations
        List<ShareholderModel> GetAllShareholders();
        ShareholderModel GetShareholderById(int id);
        ShareholderModel GetShareholderByFolioId(string folioId);
        List<ShareholderModel> SearchShareholders(string searchTerm);
        
        // Command Operations
        bool CreateShareholder(ShareholderModel shareholder);
        bool UpdateShareholder(ShareholderModel shareholder);
        bool DeleteShareholder(string folioId);
        
        // Validation
        bool ValidateFolioId(string folioId);
        int GetTotalCount();
    }
}
