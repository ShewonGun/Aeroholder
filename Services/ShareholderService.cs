using System;
using System.Collections.Generic;
using System.Linq;
using AeroHolder_new.Models;
using AeroHolder_new.Repositories;

namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service implementation for Shareholder business logic
    /// </summary>
    public class ShareholderService : IShareholderService
    {
        private readonly IShareholderRepository _repository;

        public ShareholderService(IShareholderRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public List<ShareholderModel> GetAllShareholders()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving shareholders", ex);
            }
        }

        public ShareholderModel GetShareholderById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid shareholder ID", nameof(id));

            return _repository.GetById(id);
        }

        public ShareholderModel GetShareholderByFolioId(string folioId)
        {
            if (string.IsNullOrWhiteSpace(folioId))
                throw new ArgumentException("Folio ID cannot be empty", nameof(folioId));

            return _repository.GetByFolioId(folioId);
        }

        public List<ShareholderModel> SearchShareholders(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllShareholders();

            return _repository.Search(searchTerm);
        }

        public bool CreateShareholder(ShareholderModel shareholder)
        {
            // Validation
            ValidateShareholder(shareholder);

            if (_repository.Exists(shareholder.FolioID))
                throw new Exception($"Shareholder with Folio ID '{shareholder.FolioID}' already exists");

            return _repository.Add(shareholder);
        }

        public bool UpdateShareholder(ShareholderModel shareholder)
        {
            ValidateShareholder(shareholder);

            if (!_repository.Exists(shareholder.FolioID))
                throw new Exception($"Shareholder with Folio ID '{shareholder.FolioID}' does not exist");

            return _repository.Update(shareholder);
        }

        public bool DeleteShareholder(string folioId)
        {
            if (string.IsNullOrWhiteSpace(folioId))
                throw new ArgumentException("Folio ID cannot be empty", nameof(folioId));

            if (!_repository.Exists(folioId))
                throw new Exception($"Shareholder with Folio ID '{folioId}' does not exist");

            return _repository.Delete(folioId);
        }

        public bool ValidateFolioId(string folioId)
        {
            if (string.IsNullOrWhiteSpace(folioId))
                return false;

            // Business rule: Folio ID must start with "FLN" and be followed by numbers
            if (!folioId.StartsWith("FLN"))
                return false;

            string numbers = folioId.Substring(3);
            return numbers.All(char.IsDigit) && numbers.Length > 0;
        }

        public int GetTotalCount()
        {
            return _repository.Count();
        }

        private void ValidateShareholder(ShareholderModel shareholder)
        {
            if (shareholder == null)
                throw new ArgumentNullException(nameof(shareholder));

            if (string.IsNullOrWhiteSpace(shareholder.FirstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(shareholder.LastName))
                throw new ArgumentException("Last name is required");

            if (string.IsNullOrWhiteSpace(shareholder.FolioID))
                throw new ArgumentException("Folio ID is required");

            if (!ValidateFolioId(shareholder.FolioID))
                throw new ArgumentException("Invalid Folio ID format. Must start with 'FLN' followed by numbers");

            if (shareholder.NoOfShares < 0)
                throw new ArgumentException("Number of shares cannot be negative");

            if (shareholder.NoOfTicketsIssued < 0)
                throw new ArgumentException("Number of tickets issued cannot be negative");
        }
    }
}
