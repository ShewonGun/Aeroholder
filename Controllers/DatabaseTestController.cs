using System;
using System.Web.Mvc;
using AeroHolder_new.Models;
using AeroHolder_new.Data;
using AeroHolder_new.Repositories;
using System.Data.SqlClient;

namespace AeroHolder_new.Controllers
{
    public class DatabaseTestController : Controller
    {
        // GET: /DatabaseTest
        public ActionResult Index()
        {
            return View();
        }

        // Test basic connection
        public ActionResult TestConnection()
        {
            try
            {
                var dbContext = new AppDbContext();
                using (var connection = dbContext.GetConnection())
                {
                    connection.Open();
                    
                    ViewBag.Status = "success";
                    ViewBag.Message = "? Database connection successful!";
                    ViewBag.ServerVersion = connection.ServerVersion;
                    ViewBag.Database = connection.Database;
                    ViewBag.DataSource = connection.DataSource;
                    ViewBag.State = connection.State.ToString();
                }
            }
            catch (SqlException sqlEx)
            {
                ViewBag.Status = "error";
                ViewBag.Message = "? SQL Server connection failed!";
                ViewBag.Error = sqlEx.Message;
                ViewBag.ErrorNumber = sqlEx.Number;
                
                // Provide specific error messages
                switch (sqlEx.Number)
                {
                    case 53:
                        ViewBag.Suggestion = "SQL Server not found or not accessible. Check server name in connection string.";
                        break;
                    case 4060:
                        ViewBag.Suggestion = "Database 'AeroHolderDB' does not exist. Run the SetupDatabase.sql script.";
                        break;
                    case 18456:
                        ViewBag.Suggestion = "Login failed. Check username and password in connection string.";
                        break;
                    default:
                        ViewBag.Suggestion = "Check your connection string in Web.config file.";
                        break;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = "error";
                ViewBag.Message = "? Connection failed!";
                ViewBag.Error = ex.Message;
                ViewBag.Suggestion = "Check if SQL Server is installed and running.";
            }
            
            return View("Index");
        }

        // Test query execution
        public ActionResult TestQuery()
        {
            try
            {
                var context = new AppDbContext();
                var repository = new ShareholderRepository(context);
                var shareholders = repository.GetAll();
                
                ViewBag.Status = "success";
                ViewBag.Message = $"? Successfully retrieved {shareholders.Count} shareholders from database";
                ViewBag.Shareholders = shareholders;
            }
            catch (SqlException sqlEx)
            {
                ViewBag.Status = "error";
                ViewBag.Message = "? Query execution failed!";
                ViewBag.Error = sqlEx.Message;
                
                if (sqlEx.Message.Contains("Invalid object name 'Shareholders'"))
                {
                    ViewBag.Suggestion = "The Shareholders table doesn't exist. Run the SetupDatabase.sql script.";
                }
                else
                {
                    ViewBag.Suggestion = "Check if the database schema is set up correctly.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = "error";
                ViewBag.Message = "? Query failed!";
                ViewBag.Error = ex.Message;
            }
            
            return View("Index");
        }

        // Test user login
        public ActionResult TestLogin(string username = "admin", string password = "admin123")
        {
            try
            {
                var context = new AppDbContext();
                var userRepository = new UserRepository(context);
                bool isValid = userRepository.ValidateCredentials(username, password);
                
                if (isValid)
                {
                    ViewBag.Status = "success";
                    ViewBag.Message = $"? Login successful for user: {username}";
                }
                else
                {
                    ViewBag.Status = "warning";
                    ViewBag.Message = $"?? Login failed for user: {username}";
                    ViewBag.Suggestion = "User not found or password incorrect. Try: admin/admin123 or user1/user123";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Status = "error";
                ViewBag.Message = "? Login test failed!";
                ViewBag.Error = ex.Message;
            }
            
            return View("Index");
        }

        // Show connection string (masked password)
        public ActionResult ShowConnectionString()
        {
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                
                // Mask password for security
                var maskedConnectionString = System.Text.RegularExpressions.Regex.Replace(
                    connectionString, 
                    @"Password=([^;]*)", 
                    "Password=****", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
                
                ViewBag.Status = "info";
                ViewBag.Message = "Current Connection String:";
                ViewBag.ConnectionString = maskedConnectionString;
            }
            catch (Exception ex)
            {
                ViewBag.Status = "error";
                ViewBag.Message = "? Failed to read connection string!";
                ViewBag.Error = ex.Message;
            }
            
            return View("Index");
        }
    }
}
