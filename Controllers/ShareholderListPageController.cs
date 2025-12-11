using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Models;
using AeroHolder_new.Services;
using AeroHolder_new.Helpers;

namespace AeroHolder_new.Controllers
{
    /// <summary>
    /// Controller for shareholder list management
    /// </summary>
    public class ShareholderListPageController : Controller
    {
        private readonly IShareholderService _shareholderService;
        private readonly PassportService _passportService;
        private readonly DependentService _dependentService;

        public ShareholderListPageController()
        {
            // Initialize dependencies
            var context = new AppDbContext();
            var shareholderRepository = new Repositories.ShareholderRepository(context);
            _shareholderService = new ShareholderService(shareholderRepository);
            
            var passportRepository = new Repositories.PassportRepository(context);
            _passportService = new PassportService(passportRepository);
            
            var dependentRepository = new Repositories.DependentRepository(context);
            _dependentService = new DependentService(dependentRepository);
        }

        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = Session["UserName"] ?? "User";
            
            try
            {
                var shareholders = _shareholderService.GetAllShareholders();
                return View("~/Views/ShareholderListPage/shareholderPage.cshtml", shareholders);
            }
            catch (Exception ex)
            {
                NotificationHelper.Error(TempData, $"Failed to load shareholders: {ex.Message}");
                return View("~/Views/ShareholderListPage/shareholderPage.cshtml", new List<ShareholderModel>());
            }
        }

        public ActionResult shareholderPage()
        {
            return Index();
        }

        // Search shareholders
        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = Session["UserName"] ?? "User";
            
            try
            {
                var shareholders = _shareholderService.SearchShareholders(searchTerm);
                
                if (shareholders == null || shareholders.Count == 0)
                {
                    NotificationHelper.Info(TempData, $"No shareholders found matching '{searchTerm}'");
                }
                else
                {
                    NotificationHelper.Success(TempData, $"Found {shareholders.Count} shareholder(s) matching '{searchTerm}'");
                }
                
                return View("~/Views/ShareholderListPage/shareholderPage.cshtml", shareholders);
            }
            catch (Exception ex)
            {
                NotificationHelper.Error(TempData, $"Search failed: {ex.Message}");
                return View("~/Views/ShareholderListPage/shareholderPage.cshtml", new List<ShareholderModel>());
            }
        }

        // Get shareholder details with passports and dependents (API endpoint)
        [HttpGet]
        public ActionResult GetShareholderWithDetails(string folioId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folioId))
                {
                    return Json(new { success = false, message = "Folio ID is required" }, JsonRequestBehavior.AllowGet);
                }

                var shareholder = _shareholderService.GetShareholderByFolioId(folioId);
                
                if (shareholder != null)
                {
                    // Get passports
                    var passports = _passportService.GetPassportsByShareholderID(shareholder.ShareholderID);
                    
                    // Format passport dates for JSON
                    var formattedPassports = new List<object>();
                    foreach (var passport in passports)
                    {
                        formattedPassports.Add(new
                        {
                            passport.PassportID,
                            passport.ShareholderID,
                            passport.PassportNumber,
                            passport.IssuedCountry,
                            ExpiryDate = passport.ExpiryDate.HasValue ? passport.ExpiryDate.Value.ToString("yyyy-MM-dd") : null,
                            IssuedDate = passport.IssuedDate.HasValue ? passport.IssuedDate.Value.ToString("yyyy-MM-dd") : null
                        });
                    }
                    
                    // Get dependents
                    var dependents = _dependentService.GetDependentsByShareholderID(shareholder.ShareholderID);
                    
                    return Json(new { 
                        success = true, 
                        data = new {
                            shareholder = shareholder,
                            passports = formattedPassports,
                            dependents = dependents
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Shareholder not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error loading shareholder: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get shareholder details (API endpoint)
        [HttpGet]
        public ActionResult GetShareholder(string folioId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folioId))
                {
                    return Json(new { success = false, message = "Folio ID is required" }, JsonRequestBehavior.AllowGet);
                }

                var shareholder = _shareholderService.GetShareholderByFolioId(folioId);
                
                if (shareholder != null)
                {
                    return Json(new { success = true, data = shareholder }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Shareholder not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error loading shareholder: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete shareholder
        [HttpPost]
        public ActionResult DeleteShareholder(string folioId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folioId))
                {
                    return Json(new { success = false, message = "Folio ID is required" });
                }

                bool success = _shareholderService.DeleteShareholder(folioId);
                
                if (success)
                {
                    NotificationHelper.Success(TempData, $"Shareholder {folioId} deleted successfully!");
                    return Json(new { success = true, message = "Shareholder deleted successfully" });
                }
                
                NotificationHelper.Error(TempData, $"Failed to delete shareholder {folioId}");
                return Json(new { success = false, message = "Failed to delete shareholder" });
            }
            catch (Exception ex)
            {
                NotificationHelper.Error(TempData, $"Delete failed: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Add new shareholder with passports and dependents
        [HttpPost]
        public ActionResult AddShareholder(ShareholderWithDetailsModel model)
        {
            try
            {
                // Validation
                if (model == null || model.Shareholder == null)
                {
                    NotificationHelper.Warning(TempData, "Invalid shareholder data");
                    return Json(new { success = false, message = "Invalid shareholder data", reload = true });
                }

                var shareholder = model.Shareholder;

                if (string.IsNullOrWhiteSpace(shareholder.FirstName))
                {
                    NotificationHelper.Warning(TempData, "First Name is required");
                    return Json(new { success = false, message = "First Name is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(shareholder.LastName))
                {
                    NotificationHelper.Warning(TempData, "Last Name is required");
                    return Json(new { success = false, message = "Last Name is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(shareholder.FolioID))
                {
                    NotificationHelper.Warning(TempData, "Folio ID is required");
                    return Json(new { success = false, message = "Folio ID is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(shareholder.FullName))
                {
                    NotificationHelper.Warning(TempData, "Full Name (Passport Name) is required");
                    return Json(new { success = false, message = "Full Name is required", reload = true });
                }

                // Validate FolioID format
                if (!shareholder.FolioID.StartsWith("FLN") || shareholder.FolioID.Length < 4)
                {
                    NotificationHelper.Warning(TempData, "Invalid Folio ID format. Must start with 'FLN' followed by numbers");
                    return Json(new { success = false, message = "Invalid Folio ID format", reload = true });
                }

                // Validate numbers are non-negative
                if (shareholder.NoOfShares < 0)
                {
                    NotificationHelper.Warning(TempData, "Number of shares cannot be negative");
                    return Json(new { success = false, message = "Number of shares cannot be negative", reload = true });
                }

                if (shareholder.NoOfTicketsIssued < 0)
                {
                    NotificationHelper.Warning(TempData, "Number of tickets issued cannot be negative");
                    return Json(new { success = false, message = "Number of tickets cannot be negative", reload = true });
                }

                if (shareholder.Entitlement < 0)
                {
                    NotificationHelper.Warning(TempData, "Entitlement cannot be negative");
                    return Json(new { success = false, message = "Entitlement cannot be negative", reload = true });
                }

                // Attempt to create shareholder
                bool success = _shareholderService.CreateShareholder(shareholder);
                
                if (success)
                {
                    // Get the newly added shareholder to get ShareholderID
                    var addedShareholder = _shareholderService.GetShareholderByFolioId(shareholder.FolioID);

                    if (addedShareholder != null)
                    {
                        int passportsSaved = 0;
                        int dependentsSaved = 0;

                        // Save passports
                        if (model.Passports != null && model.Passports.Count > 0)
                        {
                            foreach (var passport in model.Passports)
                            {
                                if (!string.IsNullOrWhiteSpace(passport.PassportNumber))
                                {
                                    passport.ShareholderID = addedShareholder.ShareholderID;
                                    if (_passportService.AddPassport(passport))
                                    {
                                        passportsSaved++;
                                    }
                                }
                            }
                        }

                        // Save dependents
                        if (model.Dependents != null && model.Dependents.Count > 0)
                        {
                            foreach (var dependent in model.Dependents)
                            {
                                if (!string.IsNullOrWhiteSpace(dependent.FullName))
                                {
                                    dependent.ShareholderID = addedShareholder.ShareholderID;
                                    if (_dependentService.AddDependent(dependent))
                                    {
                                        dependentsSaved++;
                                    }
                                }
                            }
                        }

                        string message = $"Shareholder {shareholder.FolioID} ({shareholder.FullName}) added successfully!";
                        if (passportsSaved > 0)
                            message += $" {passportsSaved} passport(s) added.";
                        if (dependentsSaved > 0)
                            message += $" {dependentsSaved} dependent(s) added.";

                        NotificationHelper.Success(TempData, message);
                    }
                    else
                    {
                        NotificationHelper.Success(TempData, $"Shareholder {shareholder.FolioID} ({shareholder.FullName}) added successfully!");
                    }

                    return Json(new { success = true, message = "Shareholder added successfully" });
                }
                
                NotificationHelper.Error(TempData, $"Failed to add shareholder {shareholder.FolioID}");
                return Json(new { success = false, message = "Failed to add shareholder" });
            }
            catch (ArgumentException argEx)
            {
                // Validation errors from service layer
                NotificationHelper.Warning(TempData, argEx.Message);
                return Json(new { success = false, message = argEx.Message, reload = true });
            }
            catch (Exception ex)
            {
                // Check for duplicate folio ID
                if (ex.Message.Contains("already exists"))
                {
                    NotificationHelper.Error(TempData, $"Shareholder with Folio ID already exists!");
                    return Json(new { success = false, message = ex.Message });
                }
                
                NotificationHelper.Error(TempData, $"Failed to add shareholder: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Update shareholder
        [HttpPost]
        public ActionResult UpdateShareholder(ShareholderModel model)
        {
            try
            {
                // Validation
                if (model == null)
                {
                    NotificationHelper.Warning(TempData, "Invalid shareholder data");
                    return Json(new { success = false, message = "Invalid shareholder data", reload = true });
                }

                if (string.IsNullOrWhiteSpace(model.FirstName))
                {
                    NotificationHelper.Warning(TempData, "First Name is required");
                    return Json(new { success = false, message = "First Name is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(model.LastName))
                {
                    NotificationHelper.Warning(TempData, "Last Name is required");
                    return Json(new { success = false, message = "Last Name is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(model.FolioID))
                {
                    NotificationHelper.Warning(TempData, "Folio ID is required");
                    return Json(new { success = false, message = "Folio ID is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(model.FullName))
                {
                    NotificationHelper.Warning(TempData, "Full Name (Passport Name) is required");
                    return Json(new { success = false, message = "Full Name is required", reload = true });
                }

                // Validate numbers are non-negative
                if (model.NoOfShares < 0)
                {
                    NotificationHelper.Warning(TempData, "Number of shares cannot be negative");
                    return Json(new { success = false, message = "Number of shares cannot be negative", reload = true });
                }

                if (model.NoOfTicketsIssued < 0)
                {
                    NotificationHelper.Warning(TempData, "Number of tickets issued cannot be negative");
                    return Json(new { success = false, message = "Number of tickets cannot be negative", reload = true });
                }

                if (model.Entitlement < 0)
                {
                    NotificationHelper.Warning(TempData, "Entitlement cannot be negative");
                    return Json(new { success = false, message = "Entitlement cannot be negative", reload = true });
                }

                // Attempt to update shareholder
                bool success = _shareholderService.UpdateShareholder(model);
                
                if (success)
                {
                    NotificationHelper.Success(TempData, $"Shareholder {model.FolioID} ({model.FullName}) updated successfully!");
                    return Json(new { success = true, message = "Shareholder updated successfully" });
                }
                
                NotificationHelper.Error(TempData, $"Failed to update shareholder {model.FolioID}");
                return Json(new { success = false, message = "Failed to update shareholder" });
            }
            catch (ArgumentException argEx)
            {
                // Validation errors from service layer
                NotificationHelper.Warning(TempData, argEx.Message);
                return Json(new { success = false, message = argEx.Message, reload = true });
            }
            catch (Exception ex)
            {
                // Check for not found error
                if (ex.Message.Contains("does not exist"))
                {
                    NotificationHelper.Error(TempData, $"Shareholder with Folio ID '{model.FolioID}' not found!");
                    return Json(new { success = false, message = ex.Message });
                }
                
                NotificationHelper.Error(TempData, $"Failed to update shareholder: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Update shareholder with passports and dependents
        [HttpPost]
        public ActionResult UpdateShareholderWithDetails(ShareholderWithDetailsModel model)
        {
            try
            {
                // Validation
                if (model == null || model.Shareholder == null)
                {
                    NotificationHelper.Warning(TempData, "Invalid shareholder data");
                    return Json(new { success = false, message = "Invalid shareholder data", reload = true });
                }

                var shareholder = model.Shareholder;

                if (string.IsNullOrWhiteSpace(shareholder.FirstName))
                {
                    NotificationHelper.Warning(TempData, "First Name is required");
                    return Json(new { success = false, message = "First Name is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(shareholder.LastName))
                {
                    NotificationHelper.Warning(TempData, "Last Name is required");
                    return Json(new { success = false, message = "Last Name is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(shareholder.FolioID))
                {
                    NotificationHelper.Warning(TempData, "Folio ID is required");
                    return Json(new { success = false, message = "Folio ID is required", reload = true });
                }

                if (string.IsNullOrWhiteSpace(shareholder.FullName))
                {
                    NotificationHelper.Warning(TempData, "Full Name (Passport Name) is required");
                    return Json(new { success = false, message = "Full Name is required", reload = true });
                }

                // Validate numbers are non-negative
                if (shareholder.NoOfShares < 0)
                {
                    NotificationHelper.Warning(TempData, "Number of shares cannot be negative");
                    return Json(new { success = false, message = "Number of shares cannot be negative", reload = true });
                }

                if (shareholder.NoOfTicketsIssued < 0)
                {
                    NotificationHelper.Warning(TempData, "Number of tickets issued cannot be negative");
                    return Json(new { success = false, message = "Number of tickets cannot be negative", reload = true });
                }

                if (shareholder.Entitlement < 0)
                {
                    NotificationHelper.Warning(TempData, "Entitlement cannot be negative");
                    return Json(new { success = false, message = "Entitlement cannot be negative", reload = true });
                }

                // Attempt to update shareholder
                bool success = _shareholderService.UpdateShareholder(shareholder);
                
                if (success)
                {
                    int passportsSaved = 0;
                    int dependentsSaved = 0;

                    // Delete existing passports and dependents, then add new ones
                    _passportService.DeletePassportsByShareholderID(shareholder.ShareholderID);
                    _dependentService.DeleteDependentsByShareholderID(shareholder.ShareholderID);

                    // Save passports
                    if (model.Passports != null && model.Passports.Count > 0)
                    {
                        foreach (var passport in model.Passports)
                        {
                            if (!string.IsNullOrWhiteSpace(passport.PassportNumber))
                            {
                                passport.ShareholderID = shareholder.ShareholderID;
                                if (_passportService.AddPassport(passport))
                                {
                                    passportsSaved++;
                                }
                            }
                        }
                    }

                    // Save dependents
                    if (model.Dependents != null && model.Dependents.Count > 0)
                    {
                        foreach (var dependent in model.Dependents)
                        {
                            if (!string.IsNullOrWhiteSpace(dependent.FullName))
                            {
                                dependent.ShareholderID = shareholder.ShareholderID;
                                if (_dependentService.AddDependent(dependent))
                                {
                                    dependentsSaved++;
                                }
                            }
                        }
                    }

                    string message = $"Shareholder {shareholder.FolioID} ({shareholder.FullName}) updated successfully!";
                    if (passportsSaved > 0)
                        message += $" {passportsSaved} passport(s) saved.";
                    if (dependentsSaved > 0)
                        message += $" {dependentsSaved} dependent(s) saved.";

                    NotificationHelper.Success(TempData, message);
                    return Json(new { success = true, message = "Shareholder updated successfully" });
                }
                
                NotificationHelper.Error(TempData, $"Failed to update shareholder {shareholder.FolioID}");
                return Json(new { success = false, message = "Failed to update shareholder" });
            }
            catch (ArgumentException argEx)
            {
                // Validation errors from service layer
                NotificationHelper.Warning(TempData, argEx.Message);
                return Json(new { success = false, message = argEx.Message, reload = true });
            }
            catch (Exception ex)
            {
                // Check for not found error
                if (ex.Message.Contains("does not exist"))
                {
                    NotificationHelper.Error(TempData, $"Shareholder not found!");
                    return Json(new { success = false, message = ex.Message });
                }
                
                NotificationHelper.Error(TempData, $"Failed to update shareholder: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
