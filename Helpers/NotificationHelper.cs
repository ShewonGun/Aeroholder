using System.Web.Mvc;

namespace AeroHolder_new.Helpers
{
    /// <summary>
    /// Helper class for managing notification messages using TempData
    /// </summary>
    public static class NotificationHelper
    {
        /// <summary>
        /// Sets a success notification message
        /// </summary>
        /// <param name="tempData">TempData dictionary</param>
        /// <param name="message">Success message to display</param>
        public static void Success(TempDataDictionary tempData, string message)
        {
            tempData["SuccessMessage"] = message;
        }

        /// <summary>
        /// Sets an error notification message
        /// </summary>
        /// <param name="tempData">TempData dictionary</param>
        /// <param name="message">Error message to display</param>
        public static void Error(TempDataDictionary tempData, string message)
        {
            tempData["ErrorMessage"] = message;
        }

        /// <summary>
        /// Sets a warning notification message
        /// </summary>
        /// <param name="tempData">TempData dictionary</param>
        /// <param name="message">Warning message to display</param>
        public static void Warning(TempDataDictionary tempData, string message)
        {
            tempData["WarningMessage"] = message;
        }

        /// <summary>
        /// Sets an information notification message
        /// </summary>
        /// <param name="tempData">TempData dictionary</param>
        /// <param name="message">Information message to display</param>
        public static void Info(TempDataDictionary tempData, string message)
        {
            tempData["InfoMessage"] = message;
        }
    }
}
