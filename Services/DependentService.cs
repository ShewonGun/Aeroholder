using System;
using System.Collections.Generic;
using AeroHolder_new.Models;
using AeroHolder_new.Repositories;

namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service for Dependent business logic
    /// </summary>
    public class DependentService
    {
        private readonly DependentRepository _dependentRepository;

        public DependentService(DependentRepository dependentRepository)
        {
            _dependentRepository = dependentRepository ?? throw new ArgumentNullException(nameof(dependentRepository));
        }

        /// <summary>
        /// Add a new dependent
        /// </summary>
        public bool AddDependent(DependentModel dependent)
        {
            try
            {
                if (dependent == null)
                    throw new ArgumentNullException(nameof(dependent));

                if (dependent.ShareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                // Only save if dependent has data
                if (string.IsNullOrWhiteSpace(dependent.FullName))
                    return true; // Skip empty dependents

                return _dependentRepository.Add(dependent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in DependentService.AddDependent: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all dependents for a shareholder
        /// </summary>
        public List<DependentModel> GetDependentsByShareholderID(int shareholderID)
        {
            try
            {
                if (shareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                return _dependentRepository.GetByShareholderID(shareholderID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in DependentService.GetDependentsByShareholderID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete all dependents for a shareholder
        /// </summary>
        public bool DeleteDependentsByShareholderID(int shareholderID)
        {
            try
            {
                if (shareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                return _dependentRepository.DeleteByShareholderID(shareholderID);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in DependentService.DeleteDependentsByShareholderID: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get dependent count for a shareholder
        /// </summary>
        public int GetDependentCount(int shareholderID)
        {
            try
            {
                if (shareholderID <= 0)
                    return 0;

                return _dependentRepository.GetCountByShareholderID(shareholderID);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
