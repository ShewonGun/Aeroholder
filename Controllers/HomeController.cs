using System.Web.Mvc;

namespace AeroHolder_new.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserId = Session["UserId"];
            ViewBag.LoginTime = Session["LoginTime"];
            return View();
        }
    }
}