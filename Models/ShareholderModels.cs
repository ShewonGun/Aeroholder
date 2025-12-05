using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AeroHolder_new.Models
{
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
