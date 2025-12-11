using System;
using System.Collections.Generic;

namespace AeroHolder_new.Models
{
    public class PassportModel
    {
        public int PassportID { get; set; }
        public int ShareholderID { get; set; }
        public string PassportNumber { get; set; }
        public string IssuedCountry { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DependentModel
    {
        public int DependentID { get; set; }
        public int ShareholderID { get; set; }
        public string FullName { get; set; }
        public string Relationship { get; set; }
        public string PassportNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Model for adding shareholder with related data
    /// </summary>
    public class ShareholderWithDetailsModel
    {
        public ShareholderModel Shareholder { get; set; }
        public List<PassportModel> Passports { get; set; }
        public List<DependentModel> Dependents { get; set; }

        public ShareholderWithDetailsModel()
        {
            Passports = new List<PassportModel>();
            Dependents = new List<DependentModel>();
        }
    }
}
