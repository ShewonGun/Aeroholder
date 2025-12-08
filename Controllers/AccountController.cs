using System;
using System.Web.Mvc;
using AeroHolder_new.Data;
using AeroHolder_new.Repositories;
using AeroHolder_new.Services;

namespace AeroHolder_new.Controllers
{
    /// <summary>
    /// Controller for user authentication
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AccountController()
        {
            // Initialize dependencies
            var context = new AppDbContext();
            var userRepository = new UserRepository(context);
            _authService = new AuthenticationService(userRepository);
        }

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

            try
            {
                bool isValid = _authService.AuthenticateUser(userId, password);

                if (isValid)
                {
                    if (!_authService.IsUserActive(userId))
                    {
                        ViewBag.ErrorMessage = "Your account has been deactivated. Please contact administrator.";
                        return View();
                    }

                    Session["UserId"] = userId;
                    Session["UserName"] = userId;
                    Session["LoginTime"] = DateTime.Now;
                    
                    return RedirectToAction("Index", "ShareholderListPage");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid User ID or Password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Login failed: {ex.Message}";
                return View();
            }
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        // GET: Account/TestDB
        public ActionResult TestDB()
        {
            try
            {
                var context = new AppDbContext();
                using (var connection = context.GetConnection())
                {
                    connection.Open();
                    return Content($"? SUCCESS! Connected to database: {connection.Database} on server: {connection.DataSource}");
                }
            }
            catch (Exception ex)
            {
                return Content($"? CONNECTION FAILED: {ex.Message}");
            }
        }
    }
}