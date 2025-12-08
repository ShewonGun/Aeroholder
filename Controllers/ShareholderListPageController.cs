using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Models;
using AeroHolder_new.Services;

namespace AeroHolder_new.Controllers
{
    /// <summary>
    /// Controller for shareholder list management
    /// </summary>
    public class ShareholderListPageController : Controller
    {
        private readonly IShareholderService _shareholderService;

        public ShareholderListPageController()
        {
            // Initialize dependencies
            var context = new AppDbContext();
            var repository = new Repositories.ShareholderRepository(context);
            _shareholderService = new ShareholderService(repository);
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
                ViewBag.Error = $"Failed to load shareholders: {ex.Message}";
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
                return View("~/Views/ShareholderListPage/shareholderPage.cshtml", shareholders);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Search failed: {ex.Message}";
                return View("~/Views/ShareholderListPage/shareholderPage.cshtml", new List<ShareholderModel>());
            }
        }

        // Get shareholder details (API endpoint)
        [HttpGet]
        public ActionResult GetShareholder(string folioId)
        {
            try
            {
                var shareholder = _shareholderService.GetShareholderByFolioId(folioId);
                
                if (shareholder != null)
                {
                    return Json(new { success = true, data = shareholder }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Shareholder not found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete shareholder
        [HttpPost]
        public ActionResult DeleteShareholder(string folioId)
        {
            try
            {
                bool success = _shareholderService.DeleteShareholder(folioId);
                
                if (success)
                {
                    return Json(new { success = true, message = "Shareholder deleted successfully" });
                }
                return Json(new { success = false, message = "Failed to delete shareholder" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Add new shareholder (for future implementation)
        [HttpPost]
        public ActionResult AddShareholder(ShareholderModel model)
        {
            try
            {
                bool success = _shareholderService.CreateShareholder(model);
                
                if (success)
                {
                    return Json(new { success = true, message = "Shareholder added successfully" });
                }
                return Json(new { success = false, message = "Failed to add shareholder" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Update shareholder (for future implementation)
        [HttpPost]
        public ActionResult UpdateShareholder(ShareholderModel model)
        {
            try
            {
                bool success = _shareholderService.UpdateShareholder(model);
                
                if (success)
                {
                    return Json(new { success = true, message = "Shareholder updated successfully" });
                }
                return Json(new { success = false, message = "Failed to update shareholder" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
