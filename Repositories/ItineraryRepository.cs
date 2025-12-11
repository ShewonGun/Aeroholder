using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AeroHolder_new.Data;
using AeroHolder_new.Models;

namespace AeroHolder_new.Repositories
{
    /// <summary>
    /// Repository for Itinerary and Booking History data access
    /// </summary>
    public class ItineraryRepository
    {
        private readonly AppDbContext _context;

        public ItineraryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Trip Requests

        /// <summary>
        /// Create a new trip request
        /// </summary>
        public int CreateTripRequest(TripRequestModel model)
        {
            try
            {
                string query = @"
                    INSERT INTO TripRequests 
                    (ShareholderID, FullName, TicketType, Relationship, Remarks, TicketIssue, Entitlement, Status, CreatedBy, CreatedDate, UpdatedDate)
                    VALUES 
                    (@ShareholderID, @FullName, @TicketType, @Relationship, @Remarks, @TicketIssue, @Entitlement, @Status, @CreatedBy, GETDATE(), GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlParameter[] parameters = {
                    new SqlParameter("@ShareholderID", model.ShareholderID),
                    new SqlParameter("@FullName", model.FullName ?? (object)DBNull.Value),
                    new SqlParameter("@TicketType", model.TicketType ?? (object)DBNull.Value),
                    new SqlParameter("@Relationship", model.Relationship ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", model.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@TicketIssue", model.TicketIssue ?? (object)DBNull.Value),
                    new SqlParameter("@Entitlement", model.Entitlement ?? (object)DBNull.Value),
                    new SqlParameter("@Status", "Pending"),
                    new SqlParameter("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value)
                };

                object result = _context.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating trip request: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update trip request with manage request details
        /// </summary>
        public bool UpdateTripRequest(TripRequestModel model)
        {
            try
            {
                string query = @"
                    UPDATE TripRequests 
                    SET TicketNo = @TicketNo,
                        PassportNumber = @PassportNumber,
                        DepartureAirport = @DepartureAirport,
                        DepartureCity = @DepartureCity,
                        ArrivalAirport = @ArrivalAirport,
                        ArrivalCity = @ArrivalCity,
                        DepartureDate = @DepartureDate,
                        ReturnDate = @ReturnDate,
                        Status = @Status,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = GETDATE()
                    WHERE TripRequestID = @TripRequestID";

                SqlParameter[] parameters = {
                    new SqlParameter("@TripRequestID", model.TripRequestID),
                    new SqlParameter("@TicketNo", model.TicketNo ?? (object)DBNull.Value),
                    new SqlParameter("@PassportNumber", model.PassportNumber ?? (object)DBNull.Value),
                    new SqlParameter("@DepartureAirport", model.DepartureAirport ?? (object)DBNull.Value),
                    new SqlParameter("@DepartureCity", model.DepartureCity ?? (object)DBNull.Value),
                    new SqlParameter("@ArrivalAirport", model.ArrivalAirport ?? (object)DBNull.Value),
                    new SqlParameter("@ArrivalCity", model.ArrivalCity ?? (object)DBNull.Value),
                    new SqlParameter("@DepartureDate", model.DepartureDate ?? (object)DBNull.Value),
                    new SqlParameter("@ReturnDate", model.ReturnDate ?? (object)DBNull.Value),
                    new SqlParameter("@Status", model.Status ?? "Approved"),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value)
                };

                return _context.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating trip request: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get trip request by ID
        /// </summary>
        public TripRequestModel GetTripRequestById(int tripRequestId)
        {
            try
            {
                string query = @"
                    SELECT * FROM TripRequests 
                    WHERE TripRequestID = @TripRequestID";

                SqlParameter[] parameters = { new SqlParameter("@TripRequestID", tripRequestId) };
                DataTable dt = _context.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    return MapTripRequestDataRow(dt.Rows[0]);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting trip request: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all trip requests for a shareholder
        /// </summary>
        public List<TripRequestModel> GetTripRequestsByShareholderId(int shareholderId)
        {
            try
            {
                string query = @"
                    SELECT * FROM TripRequests 
                    WHERE ShareholderID = @ShareholderID
                    ORDER BY CreatedDate DESC";

                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderId) };
                DataTable dt = _context.ExecuteQuery(query, parameters);

                List<TripRequestModel> requests = new List<TripRequestModel>();
                foreach (DataRow row in dt.Rows)
                {
                    requests.Add(MapTripRequestDataRow(row));
                }
                return requests;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting trip requests: {ex.Message}", ex);
            }
        }

        #endregion

        #region Booking History

        /// <summary>
        /// Create a new booking history record
        /// </summary>
        public int CreateBookingHistory(BookingHistoryModel model)
        {
            try
            {
                string query = @"
                    INSERT INTO BookingHistory 
                    (ShareholderID, TripRequestID, TicketNo, PassengerName, TicketType, DepartureAirport, ArrivalAirport, 
                     DepartureDate, ReturnDate, BookingDate, Status, UpdatedBy, CreatedDate, UpdatedDate)
                    VALUES 
                    (@ShareholderID, @TripRequestID, @TicketNo, @PassengerName, @TicketType, @DepartureAirport, @ArrivalAirport,
                     @DepartureDate, @ReturnDate, @BookingDate, @Status, @UpdatedBy, GETDATE(), GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlParameter[] parameters = {
                    new SqlParameter("@ShareholderID", model.ShareholderID),
                    new SqlParameter("@TripRequestID", model.TripRequestID ?? (object)DBNull.Value),
                    new SqlParameter("@TicketNo", model.TicketNo ?? (object)DBNull.Value),
                    new SqlParameter("@PassengerName", model.PassengerName ?? (object)DBNull.Value),
                    new SqlParameter("@TicketType", model.TicketType ?? (object)DBNull.Value),
                    new SqlParameter("@DepartureAirport", model.DepartureAirport ?? (object)DBNull.Value),
                    new SqlParameter("@ArrivalAirport", model.ArrivalAirport ?? (object)DBNull.Value),
                    new SqlParameter("@DepartureDate", model.DepartureDate ?? (object)DBNull.Value),
                    new SqlParameter("@ReturnDate", model.ReturnDate ?? (object)DBNull.Value),
                    new SqlParameter("@BookingDate", model.BookingDate ?? DateTime.Now),
                    new SqlParameter("@Status", model.Status ?? "Issued"),
                    new SqlParameter("@UpdatedBy", model.UpdatedBy ?? (object)DBNull.Value)
                };

                object result = _context.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating booking history: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get booking history for a shareholder
        /// </summary>
        public List<BookingHistoryModel> GetBookingHistoryByShareholderId(int shareholderId)
        {
            try
            {
                string query = @"
                    SELECT * FROM BookingHistory 
                    WHERE ShareholderID = @ShareholderID
                    ORDER BY CreatedDate DESC";

                SqlParameter[] parameters = { new SqlParameter("@ShareholderID", shareholderId) };
                DataTable dt = _context.ExecuteQuery(query, parameters);

                List<BookingHistoryModel> bookings = new List<BookingHistoryModel>();
                foreach (DataRow row in dt.Rows)
                {
                    bookings.Add(MapBookingHistoryDataRow(row));
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting booking history: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search booking history
        /// </summary>
        public List<BookingHistoryModel> SearchBookingHistory(int shareholderId, string searchTerm)
        {
            try
            {
                string query = @"
                    SELECT * FROM BookingHistory 
                    WHERE ShareholderID = @ShareholderID
                    AND (PassengerName LIKE @SearchTerm 
                         OR TicketNo LIKE @SearchTerm
                         OR UpdatedBy LIKE @SearchTerm)
                    ORDER BY CreatedDate DESC";

                SqlParameter[] parameters = {
                    new SqlParameter("@ShareholderID", shareholderId),
                    new SqlParameter("@SearchTerm", "%" + searchTerm + "%")
                };

                DataTable dt = _context.ExecuteQuery(query, parameters);

                List<BookingHistoryModel> bookings = new List<BookingHistoryModel>();
                foreach (DataRow row in dt.Rows)
                {
                    bookings.Add(MapBookingHistoryDataRow(row));
                }
                return bookings;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching booking history: {ex.Message}", ex);
            }
        }

        #endregion

        #region Helper Methods

        private TripRequestModel MapTripRequestDataRow(DataRow row)
        {
            return new TripRequestModel
            {
                TripRequestID = Convert.ToInt32(row["TripRequestID"]),
                ShareholderID = Convert.ToInt32(row["ShareholderID"]),
                FullName = row["FullName"]?.ToString(),
                TicketType = row["TicketType"]?.ToString(),
                Relationship = row["Relationship"]?.ToString(),
                Remarks = row["Remarks"]?.ToString(),
                TicketIssue = row["TicketIssue"] != DBNull.Value ? Convert.ToInt32(row["TicketIssue"]) : (int?)null,
                Entitlement = row["Entitlement"]?.ToString(),
                TicketNo = row["TicketNo"]?.ToString(),
                PassportNumber = row["PassportNumber"]?.ToString(),
                DepartureAirport = row["DepartureAirport"]?.ToString(),
                DepartureCity = row["DepartureCity"]?.ToString(),
                ArrivalAirport = row["ArrivalAirport"]?.ToString(),
                ArrivalCity = row["ArrivalCity"]?.ToString(),
                DepartureDate = row["DepartureDate"] != DBNull.Value ? Convert.ToDateTime(row["DepartureDate"]) : (DateTime?)null,
                ReturnDate = row["ReturnDate"] != DBNull.Value ? Convert.ToDateTime(row["ReturnDate"]) : (DateTime?)null,
                Status = row["Status"]?.ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                UpdatedDate = Convert.ToDateTime(row["UpdatedDate"]),
                CreatedBy = row["CreatedBy"]?.ToString(),
                UpdatedBy = row["UpdatedBy"]?.ToString()
            };
        }

        private BookingHistoryModel MapBookingHistoryDataRow(DataRow row)
        {
            return new BookingHistoryModel
            {
                BookingID = Convert.ToInt32(row["BookingID"]),
                ShareholderID = Convert.ToInt32(row["ShareholderID"]),
                TripRequestID = row["TripRequestID"] != DBNull.Value ? Convert.ToInt32(row["TripRequestID"]) : (int?)null,
                TicketNo = row["TicketNo"]?.ToString(),
                PassengerName = row["PassengerName"]?.ToString(),
                TicketType = row["TicketType"]?.ToString(),
                DepartureAirport = row["DepartureAirport"]?.ToString(),
                ArrivalAirport = row["ArrivalAirport"]?.ToString(),
                DepartureDate = row["DepartureDate"] != DBNull.Value ? Convert.ToDateTime(row["DepartureDate"]) : (DateTime?)null,
                ReturnDate = row["ReturnDate"] != DBNull.Value ? Convert.ToDateTime(row["ReturnDate"]) : (DateTime?)null,
                BookingDate = row["BookingDate"] != DBNull.Value ? Convert.ToDateTime(row["BookingDate"]) : (DateTime?)null,
                Status = row["Status"]?.ToString(),
                UpdatedBy = row["UpdatedBy"]?.ToString(),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                UpdatedDate = Convert.ToDateTime(row["UpdatedDate"])
            };
        }

        #endregion
    }
}
