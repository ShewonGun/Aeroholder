using System;
using System.Collections.Generic;
using AeroHolder_new.Models;
using AeroHolder_new.Repositories;

namespace AeroHolder_new.Services
{
    /// <summary>
    /// Service for Itinerary business logic
    /// </summary>
    public class ItineraryService
    {
        private readonly ItineraryRepository _itineraryRepository;

        public ItineraryService(ItineraryRepository itineraryRepository)
        {
            _itineraryRepository = itineraryRepository ?? throw new ArgumentNullException(nameof(itineraryRepository));
        }

        #region Trip Requests

        /// <summary>
        /// Create a new trip request (Form 1)
        /// </summary>
        public int CreateTripRequest(TripRequestModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (model.ShareholderID <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                if (string.IsNullOrWhiteSpace(model.FullName))
                    throw new ArgumentException("Full Name is required");

                if (string.IsNullOrWhiteSpace(model.TicketType))
                    throw new ArgumentException("Ticket Type is required");

                return _itineraryRepository.CreateTripRequest(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.CreateTripRequest: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update trip request with manage request details (Form 2)
        /// </summary>
        public bool UpdateTripRequest(TripRequestModel model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (model.TripRequestID <= 0)
                    throw new ArgumentException("Valid TripRequestID is required");

                return _itineraryRepository.UpdateTripRequest(model);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.UpdateTripRequest: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get trip request by ID
        /// </summary>
        public TripRequestModel GetTripRequestById(int tripRequestId)
        {
            try
            {
                if (tripRequestId <= 0)
                    throw new ArgumentException("Valid TripRequestID is required");

                return _itineraryRepository.GetTripRequestById(tripRequestId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.GetTripRequestById: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get all trip requests for a shareholder
        /// </summary>
        public List<TripRequestModel> GetTripRequestsByShareholderId(int shareholderId)
        {
            try
            {
                if (shareholderId <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                return _itineraryRepository.GetTripRequestsByShareholderId(shareholderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.GetTripRequestsByShareholderId: {ex.Message}", ex);
            }
        }

        #endregion

        #region Booking History

        /// <summary>
        /// Create a booking history record from trip request
        /// </summary>
        public int CreateBookingFromTripRequest(TripRequestModel tripRequest, string updatedBy)
        {
            try
            {
                if (tripRequest == null)
                    throw new ArgumentNullException(nameof(tripRequest));

                var booking = new BookingHistoryModel
                {
                    ShareholderID = tripRequest.ShareholderID,
                    TripRequestID = tripRequest.TripRequestID,
                    TicketNo = tripRequest.TicketNo,
                    PassengerName = tripRequest.FullName,
                    TicketType = tripRequest.TicketType,
                    Relationship = tripRequest.Relationship,
                    TicketIssue = tripRequest.TicketIssue,
                    Entitlement = tripRequest.Entitlement,
                    PassportNumber = tripRequest.PassportNumber,
                    DepartureAirport = tripRequest.DepartureAirport,
                    ArrivalAirport = tripRequest.ArrivalAirport,
                    DepartureDate = tripRequest.DepartureDate,
                    ReturnDate = tripRequest.ReturnDate,
                    BookingDate = DateTime.Now,
                    Status = "Issued",
                    UpdatedBy = updatedBy
                };

                return _itineraryRepository.CreateBookingHistory(booking);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.CreateBookingFromTripRequest: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get booking history for a shareholder
        /// </summary>
        public List<BookingHistoryModel> GetBookingHistoryByShareholderId(int shareholderId)
        {
            try
            {
                if (shareholderId <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                return _itineraryRepository.GetBookingHistoryByShareholderId(shareholderId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.GetBookingHistoryByShareholderId: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search booking history
        /// </summary>
        public List<BookingHistoryModel> SearchBookingHistory(int shareholderId, string searchTerm)
        {
            try
            {
                if (shareholderId <= 0)
                    throw new ArgumentException("Valid ShareholderID is required");

                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetBookingHistoryByShareholderId(shareholderId);

                return _itineraryRepository.SearchBookingHistory(shareholderId, searchTerm);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in ItineraryService.SearchBookingHistory: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
