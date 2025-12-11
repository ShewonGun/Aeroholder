using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Models;
using AeroHolder_new.Services;
using AeroHolder_new.Repositories;
using AeroHolder_new.Helpers;

namespace AeroHolder_new.Controllers
{
    public class ItineraryController : Controller
    {
        private readonly IShareholderService _shareholderService;
        private readonly ItineraryService _itineraryService;

        public ItineraryController()
        {
            var context = new AppDbContext();
            var shareholderRepository = new ShareholderRepository(context);
            _shareholderService = new ShareholderService(shareholderRepository);
            
            var itineraryRepository = new ItineraryRepository(context);
            _itineraryService = new ItineraryService(itineraryRepository);
        }

        public ActionResult Index(string folioId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(folioId))
            {
                return RedirectToAction("Index", "ShareholderListPage");
            }

            var shareholder = _shareholderService.GetShareholderByFolioId(folioId);
            
            if (shareholder == null)
            {
                return RedirectToAction("Index", "ShareholderListPage");
            }

            ViewBag.ShareholderName = shareholder.FullName;
            ViewBag.FolioId = shareholder.FolioID;

            return View("~/Views/ItineraryPage/itineraryPage.cshtml");
        }

        #region Trip Request API Endpoints

        /// <summary>
        /// Submit Trip Request Form 1
        /// </summary>
        [HttpPost]
        public ActionResult SubmitTripRequest(TripRequestForm1Model formData)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, message = "Session expired. Please login again." });
                }

                if (formData == null)
                {
                    return Json(new { success = false, message = "Invalid form data" });
                }

                // Get shareholder by FolioID
                var shareholder = _shareholderService.GetShareholderByFolioId(formData.FolioID);
                if (shareholder == null)
                {
                    return Json(new { success = false, message = "Shareholder not found" });
                }

                // Create trip request model
                var tripRequest = new TripRequestModel
                {
                    ShareholderID = shareholder.ShareholderID,
                    FullName = formData.FullName,
                    TicketType = formData.TicketType,
                    Relationship = formData.Relationship,
                    Remarks = formData.Remarks,
                    TicketIssue = formData.TicketIssue,
                    Entitlement = formData.Entitlement,
                    Status = "Pending",
                    CreatedBy = Session["UserName"]?.ToString()
                };

                // Save to database
                int tripRequestId = _itineraryService.CreateTripRequest(tripRequest);

                if (tripRequestId > 0)
                {
                    return Json(new { 
                        success = true, 
                        message = "Trip request submitted successfully",
                        tripRequestId = tripRequestId
                    });
                }

                return Json(new { success = false, message = "Failed to submit trip request" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Submit Manage Request Form 2
        /// </summary>
        [HttpPost]
        public ActionResult SubmitManageRequest(ManageRequestForm2Model formData)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, message = "Session expired. Please login again." });
                }

                if (formData == null || formData.TripRequestID <= 0)
                {
                    return Json(new { success = false, message = "Invalid form data" });
                }

                // Get existing trip request
                var tripRequest = _itineraryService.GetTripRequestById(formData.TripRequestID);
                if (tripRequest == null)
                {
                    return Json(new { success = false, message = "Trip request not found" });
                }

                // Update with manage request details
                tripRequest.TicketNo = formData.TicketNo;
                tripRequest.PassportNumber = formData.PassportNumber;
                tripRequest.DepartureAirport = formData.DepartureAirport;
                tripRequest.DepartureCity = formData.DepartureCity;
                tripRequest.ArrivalAirport = formData.ArrivalAirport;
                tripRequest.ArrivalCity = formData.ArrivalCity;
                
                // Parse dates
                if (!string.IsNullOrWhiteSpace(formData.DepartureDate))
                {
                    tripRequest.DepartureDate = DateTime.Parse(formData.DepartureDate);
                }
                if (!string.IsNullOrWhiteSpace(formData.ReturnDate))
                {
                    tripRequest.ReturnDate = DateTime.Parse(formData.ReturnDate);
                }

                tripRequest.Status = "Approved";
                tripRequest.UpdatedBy = Session["UserName"]?.ToString();

                // Update trip request
                bool updated = _itineraryService.UpdateTripRequest(tripRequest);

                if (updated)
                {
                    // Create booking history record
                    int bookingId = _itineraryService.CreateBookingFromTripRequest(
                        tripRequest, 
                        Session["UserName"]?.ToString()
                    );

                    if (bookingId > 0)
                    {
                        NotificationHelper.Success(TempData, "Trip request saved and booking created successfully!");
                        return Json(new { 
                            success = true, 
                            message = "Trip request saved successfully",
                            bookingId = bookingId
                        });
                    }
                }

                return Json(new { success = false, message = "Failed to save trip request" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        #endregion

        #region Booking History API Endpoints

        /// <summary>
        /// Get booking history for a shareholder
        /// </summary>
        [HttpGet]
        public ActionResult GetBookingHistory(string folioId)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(folioId))
                {
                    return Json(new { success = false, message = "Folio ID is required" }, JsonRequestBehavior.AllowGet);
                }

                var shareholder = _shareholderService.GetShareholderByFolioId(folioId);
                if (shareholder == null)
                {
                    return Json(new { success = false, message = "Shareholder not found" }, JsonRequestBehavior.AllowGet);
                }

                var bookings = _itineraryService.GetBookingHistoryByShareholderId(shareholder.ShareholderID);

                return Json(new { 
                    success = true, 
                    data = bookings 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Search booking history
        /// </summary>
        [HttpGet]
        public ActionResult SearchBookingHistory(string folioId, string searchTerm)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(folioId))
                {
                    return Json(new { success = false, message = "Folio ID is required" }, JsonRequestBehavior.AllowGet);
                }

                var shareholder = _shareholderService.GetShareholderByFolioId(folioId);
                if (shareholder == null)
                {
                    return Json(new { success = false, message = "Shareholder not found" }, JsonRequestBehavior.AllowGet);
                }

                var bookings = _itineraryService.SearchBookingHistory(shareholder.ShareholderID, searchTerm);

                return Json(new { 
                    success = true, 
                    data = bookings 
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
