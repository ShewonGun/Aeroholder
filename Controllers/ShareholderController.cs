using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AeroHolder_new.Controllers
{
    public class ShareholderController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.UserName = Session["UserName"] ?? "Pramodh Lihinikaduwa";
            return View("~/Views/ShareholderListPage/shareholderPage.cshtml");
        }

        public ActionResult ShareholderList()
        {
            ViewBag.UserName = Session["UserName"] ?? "Pramodh Lihinikaduwa";
            return View("~/Views/ShareholderListPage/shareholderPage.cshtml");
        }
    }
}
