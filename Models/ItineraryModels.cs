using System;

namespace AeroHolder_new.Models
{
    /// <summary>
    /// Model for Trip Request data
    /// </summary>
    public class TripRequestModel
    {
        public int TripRequestID { get; set; }
        public int ShareholderID { get; set; }
        public string FullName { get; set; }
        public string TicketType { get; set; }
        public string Relationship { get; set; }
        public string Remarks { get; set; }
        public int? TicketIssue { get; set; }
        public string Entitlement { get; set; }
        
        // Manage Request Fields
        public string TicketNo { get; set; }
        public string PassportNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalAirport { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// Model for Booking History data
    /// </summary>
    public class BookingHistoryModel
    {
        public int BookingID { get; set; }
        public int ShareholderID { get; set; }
        public int? TripRequestID { get; set; }
        public string TicketNo { get; set; }
        public string PassengerName { get; set; }
        public string TicketType { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    /// <summary>
    /// Model for Trip Request Form 1 submission
    /// </summary>
    public class TripRequestForm1Model
    {
        public string FolioID { get; set; }
        public string FullName { get; set; }
        public string TicketType { get; set; }
        public string Relationship { get; set; }
        public string Remarks { get; set; }
        public int TicketIssue { get; set; }
        public string Entitlement { get; set; }
    }

    /// <summary>
    /// Model for Manage Request Form 2 submission
    /// </summary>
    public class ManageRequestForm2Model
    {
        public int TripRequestID { get; set; }
        public string TicketNo { get; set; }
        public string PassportNumber { get; set; }
        public string DepartureAirport { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalAirport { get; set; }
        public string ArrivalCity { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
