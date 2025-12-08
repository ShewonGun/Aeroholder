using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Models;
using AeroHolder_new.Services;

namespace AeroHolder_new.Controllers
{
    /// <summary>
    /// Controller for shareholder operations
    /// </summary>
    public class ShareholderController : Controller
    {
        private readonly IShareholderService _shareholderService;

        public ShareholderController()
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

        public ActionResult ShareholderList()
        {
            return Index();
        }
    }
}
