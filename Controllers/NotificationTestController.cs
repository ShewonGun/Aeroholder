using System.Web.Mvc;
using AeroHolder_new.Helpers;

namespace AeroHolder_new.Controllers
{
    /// <summary>
    /// Controller for testing notification system
    /// </summary>
    public class NotificationTestController : Controller
    {
        // GET: NotificationTest
        public ActionResult Index()
        {
            return View();
        }

        // Test Success Notification
        public ActionResult TestSuccess()
        {
            NotificationHelper.Success(TempData, "This is a success notification! The operation completed successfully.");
            return RedirectToAction("Index");
        }

        // Test Error Notification
        public ActionResult TestError()
        {
            NotificationHelper.Error(TempData, "This is an error notification! Something went wrong.");
            return RedirectToAction("Index");
        }

        // Test Warning Notification
        public ActionResult TestWarning()
        {
            NotificationHelper.Warning(TempData, "This is a warning notification! Please be aware of this issue.");
            return RedirectToAction("Index");
        }

        // Test Info Notification
        public ActionResult TestInfo()
        {
            NotificationHelper.Info(TempData, "This is an info notification! Here's some information for you.");
            return RedirectToAction("Index");
        }

        // Test All Notifications at Once
        public ActionResult TestAll()
        {
            NotificationHelper.Success(TempData, "Success notification test!");
            return RedirectToAction("Index");
        }
    }
}
