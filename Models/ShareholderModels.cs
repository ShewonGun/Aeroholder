using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AeroHolder_new.Models
{
    public class ShareholderModel
    {
        public int ShareholderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string FolioID { get; set; }
        public string Country { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string CompanyIndividual { get; set; }
        public int NoOfShares { get; set; }
        public int NoOfTicketsIssued { get; set; }
        public int Entitlement { get; set; }
        public int NoOfPassports { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class ShareholderRowModel
    {
        public int RowNumber { get; set; }
        public string FolioID { get; set; }
        public string Name { get; set; }
        public int TicketsIssued { get; set; }
        public int Passports { get; set; }
    }

    public class PaginationModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
