using System.Web.Mvc;

namespace AeroHolder_new.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string userId, string password)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Please enter both User ID and Password";
                return View();
            }

            // TODO: Replace with actual authentication logic
            // For demo purposes, accept any non-empty credentials
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password))
            {
                Session["UserId"] = userId;
                Session["LoginTime"] = System.DateTime.Now;
                
                // Redirect to home page or dashboard
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Invalid User ID or Password";
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}