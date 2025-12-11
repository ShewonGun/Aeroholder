using System;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Models;
using AeroHolder_new.Repositories;
using AeroHolder_new.Services;

namespace AeroHolder_new.Controllers
{
    /// <summary>
    /// Debug controller to test itinerary functionality
    /// </summary>
    public class ItineraryDebugController : Controller
    {
        [HttpGet]
        public ActionResult TestDatabase()
        {
            var results = new System.Text.StringBuilder();
            
            try
            {
                results.AppendLine("=== ITINERARY DEBUG TEST ===\n");
                
                // Test 1: Database Connection
                results.AppendLine("1. Testing Database Connection...");
                var context = new AppDbContext();
                var testQuery = "SELECT @@VERSION AS Version";
                var dt = context.ExecuteQuery(testQuery);
                results.AppendLine($"? Database Connected: {dt.Rows[0]["Version"]}\n");
                
                // Test 2: Check if tables exist
                results.AppendLine("2. Checking if Itinerary Tables Exist...");
                var tableCheckQuery = @"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME IN ('TripRequests', 'BookingHistory')";
                var tables = context.ExecuteQuery(tableCheckQuery);
                
                if (tables.Rows.Count == 2)
                {
                    results.AppendLine("? Both tables exist (TripRequests, BookingHistory)\n");
                }
                else
                {
                    results.AppendLine($"? ERROR: Only {tables.Rows.Count} tables found. Run ItineraryTables.sql!\n");
                    return Content(results.ToString(), "text/plain");
                }
                
                // Test 3: Check Shareholders table
                results.AppendLine("3. Checking Shareholders...");
                var shareholderQuery = "SELECT TOP 1 ShareholderID, FolioID, FullName FROM Shareholders";
                var shareholders = context.ExecuteQuery(shareholderQuery);
                
                if (shareholders.Rows.Count > 0)
                {
                    var testShareholderId = Convert.ToInt32(shareholders.Rows[0]["ShareholderID"]);
                    var testFolioId = shareholders.Rows[0]["FolioID"].ToString();
                    var testName = shareholders.Rows[0]["FullName"].ToString();
                    results.AppendLine($"? Found shareholder: {testName} (ID: {testShareholderId}, FolioID: {testFolioId})\n");
                    
                    // Test 4: Try to insert a test trip request
                    results.AppendLine("4. Testing Trip Request Insert...");
                    try
                    {
                        var itineraryRepo = new ItineraryRepository(context);
                        var testTrip = new TripRequestModel
                        {
                            ShareholderID = testShareholderId,
                            FullName = "Test Passenger",
                            TicketType = "Dependent",
                            Relationship = "Test",
                            Remarks = "Debug Test",
                            TicketIssue = 1,
                            Entitlement = "01",
                            Status = "Pending",
                            CreatedBy = "DebugTest"
                        };
                        
                        int tripId = itineraryRepo.CreateTripRequest(testTrip);
                        results.AppendLine($"? Trip Request Created with ID: {tripId}\n");
                        
                        // Test 5: Update the trip request
                        results.AppendLine("5. Testing Trip Request Update...");
                        testTrip.TripRequestID = tripId;
                        testTrip.TicketNo = "TEST123456";
                        testTrip.PassportNumber = "P1234567";
                        testTrip.DepartureAirport = "CMB";
                        testTrip.DepartureCity = "Colombo";
                        testTrip.ArrivalAirport = "DXB";
                        testTrip.ArrivalCity = "Dubai";
                        testTrip.DepartureDate = DateTime.Now.AddDays(30);
                        testTrip.ReturnDate = DateTime.Now.AddDays(40);
                        testTrip.Status = "Approved";
                        testTrip.UpdatedBy = "DebugTest";
                        
                        bool updated = itineraryRepo.UpdateTripRequest(testTrip);
                        results.AppendLine($"? Trip Request Updated: {updated}\n");
                        
                        // Test 6: Create booking history
                        results.AppendLine("6. Testing Booking History Insert...");
                        var itineraryService = new ItineraryService(itineraryRepo);
                        int bookingId = itineraryService.CreateBookingFromTripRequest(testTrip, "DebugTest");
                        results.AppendLine($"? Booking Created with ID: {bookingId}\n");
                        
                        // Test 7: Retrieve data
                        results.AppendLine("7. Testing Data Retrieval...");
                        var bookings = itineraryRepo.GetBookingHistoryByShareholderId(testShareholderId);
                        results.AppendLine($"? Retrieved {bookings.Count} booking(s)\n");
                        
                        if (bookings.Count > 0)
                        {
                            results.AppendLine("   Sample booking:");
                            var b = bookings[0];
                            results.AppendLine($"   - Passenger: {b.PassengerName}");
                            results.AppendLine($"   - Route: {b.DepartureAirport} ? {b.ArrivalAirport}");
                            results.AppendLine($"   - Status: {b.Status}");
                            results.AppendLine($"   - Created: {b.CreatedDate}\n");
                        }
                        
                        results.AppendLine("=== ALL TESTS PASSED ? ===\n");
                        results.AppendLine("If you see this, your backend is working correctly!");
                        results.AppendLine("The issue is likely in the JavaScript not calling the endpoints.\n");
                        results.AppendLine("Next steps:");
                        results.AppendLine("1. Replace JavaScript in itineraryModal.cshtml with code from JAVASCRIPT_UPDATE.txt");
                        results.AppendLine("2. Open browser console (F12) and check for errors");
                        results.AppendLine("3. Look for fetch() calls to /Itinerary/SubmitTripRequest");
                    }
                    catch (Exception ex)
                    {
                        results.AppendLine($"? ERROR during testing: {ex.Message}");
                        results.AppendLine($"Stack trace: {ex.StackTrace}");
                    }
                }
                else
                {
                    results.AppendLine("? ERROR: No shareholders found in database!\n");
                }
            }
            catch (Exception ex)
            {
                results.AppendLine($"\n? CRITICAL ERROR: {ex.Message}");
                results.AppendLine($"Stack trace: {ex.StackTrace}");
            }
            
            return Content(results.ToString(), "text/plain");
        }
        
        [HttpPost]
        public ActionResult TestDirectInsert()
        {
            try
            {
                var context = new AppDbContext();
                
                // Get first shareholder
                var shareholderQuery = "SELECT TOP 1 ShareholderID FROM Shareholders";
                var dt = context.ExecuteQuery(shareholderQuery);
                
                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "No shareholders found" });
                }
                
                int shareholderId = Convert.ToInt32(dt.Rows[0]["ShareholderID"]);
                
                // Direct SQL insert
                var insertQuery = @"
                    INSERT INTO TripRequests 
                    (ShareholderID, FullName, TicketType, Relationship, Remarks, TicketIssue, Entitlement, Status, CreatedBy, CreatedDate, UpdatedDate)
                    VALUES 
                    (@ShareholderID, @FullName, @TicketType, @Relationship, @Remarks, @TicketIssue, @Entitlement, @Status, @CreatedBy, GETDATE(), GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@ShareholderID", shareholderId),
                    new System.Data.SqlClient.SqlParameter("@FullName", "Direct Test"),
                    new System.Data.SqlClient.SqlParameter("@TicketType", "Test Type"),
                    new System.Data.SqlClient.SqlParameter("@Relationship", "Test"),
                    new System.Data.SqlClient.SqlParameter("@Remarks", "Direct SQL Test"),
                    new System.Data.SqlClient.SqlParameter("@TicketIssue", 1),
                    new System.Data.SqlClient.SqlParameter("@Entitlement", "01"),
                    new System.Data.SqlClient.SqlParameter("@Status", "Pending"),
                    new System.Data.SqlClient.SqlParameter("@CreatedBy", "DirectTest")
                };
                
                var result = context.ExecuteScalar(insertQuery, parameters);
                int tripId = Convert.ToInt32(result);
                
                return Json(new { success = true, message = $"Trip request created with ID: {tripId}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
