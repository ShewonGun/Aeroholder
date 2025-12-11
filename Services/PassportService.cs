using System;
using System.Collections.Generic;
using AeroHolder_new.Models;
using AeroHolder_new.Repositories;

namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service for Passport business logic
    /// </summary>
    public class PassportService
    {
        private readonly PassportRepository _passportRepository;

        public PassportService(PassportRepository passportRepository)
        {
            _passportRepository = passportRepository ?? throw new ArgumentNullException(nameof(passportRepository));
        }

        /// <summary>
        /// Add a new passport
        /// </summary>
        public bool AddPassport(PassportModel passport)
        {
            try
            {
                if (passport == null)
                    throw new ArgumentNullException(nameof(passport));

                if (passport.ShareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                // Only save if passport has data
                if (string.IsNullOrWhiteSpace(passport.PassportNumber))
                    return true; // Skip empty passports

                return _passportRepository.Add(passport);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PassportService.AddPassport: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all passports for a shareholder
        /// </summary>
        public List<PassportModel> GetPassportsByShareholderID(int shareholderID)
        {
            try
            {
                if (shareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                return _passportRepository.GetByShareholderID(shareholderID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PassportService.GetPassportsByShareholderID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete all passports for a shareholder
        /// </summary>
        public bool DeletePassportsByShareholderID(int shareholderID)
        {
            try
            {
                if (shareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                return _passportRepository.DeleteByShareholderID(shareholderID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PassportService.DeletePassportsByShareholderID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get passport count for a shareholder
        /// </summary>
        public int GetPassportCount(int shareholderID)
        {
            try
            {
                if (shareholderID <= 0)
                    return 0;

                return _passportRepository.GetCountByShareholderID(shareholderID);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
