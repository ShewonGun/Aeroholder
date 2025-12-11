using System;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Services;

namespace AeroHolder_new.Controllers
{
    public class ItineraryController : Controller
    {
        private readonly IShareholderService _shareholderService;

        public ItineraryController()
        {
            var context = new AppDbContext();
            var shareholderRepository = new Repositories.ShareholderRepository(context);
            _shareholderService = new ShareholderService(shareholderRepository);
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
    }
}
