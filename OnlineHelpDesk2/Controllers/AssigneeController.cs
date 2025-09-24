using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk2.Controllers
{
    [AssigneeOnly]
    public class AssigneeController : Controller
    {
        private OHDDBContext db = new OHDDBContext();
        string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
        // GET: Assignee
        [HttpGet]
        public ActionResult Index()
        {
            int accountID = (int)Session["AccountID"];

            if (Session["AccountID"] != null)
            {
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }

            int faciID = (int)Session["FaciID"];
            string query = @"
    SELECT 
        R.RequestID,
        R.RequestorID,
        RA.FullName AS RequestorFullName,
        F.FacilityName,
        R.RequestDate,
        R.AssigneeID,
        AA.FullName AS AssigneeFullName,
        R.Status,
        R.FacilityID,
        R.RequestContent,
        R.SeverityLevels
    FROM Requests R
    INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
    INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
    INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
    WHERE R.AssigneeID = @accountID and R.FacilityID = @faciID AND R.Status ='In Progress'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@faciID", faciID);
                command.Parameters.AddWithValue("@accountID", accountID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    model.Add(new RequestHistory
                    {
                        RequestID = (int)reader["RequestID"],
                        RequestorID = (int)reader["RequestorID"],
                        RequestorFullName = reader["RequestorFullName"].ToString(),
                        FacilityName = reader["FacilityName"].ToString(),
                        RequestDate = (DateTime)reader["RequestDate"],
                        AssigneeID = (int)reader["AssigneeID"],
                        AssigneeFullName = reader["AssigneeFullName"].ToString(),
                        Status = reader["Status"].ToString(),
                        RequestContent = reader["RequestContent"].ToString(),
                        SeverityLevels = reader["SeverityLevels"].ToString()
                    });
                }
            }
            int faciID1 = (int)Session["FaciID"];
            var assignees = db.Accounts.Where(u => u.TypeID == 4 && u.FacilityID == faciID1).ToList();

            ViewBag.Assignees = assignees;

            return View(model);
        }

        [HttpGet]
        public ActionResult ReplyToRequest(int requestId)
        {
            var request = db.Requests.Find(requestId);

            if (request == null)
            {
                return PartialView("ReplyToRequest", null);
            }

            if (!CanUserAccessRequest(request))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return PartialView("ReplyToRequest", request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyToRequest(int requestId, string replyContent)
        {
            var request = db.Requests.Find(requestId);

            if (request == null)
            {
                return PartialView("ReplyToRequest", null);
            }

            // Check if the user has the right to change the status
            if (!CanUserChangeStatus(request))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Change the status to "Completed"
            request.Status = "Completed";

            var reply = new Reply
            {
                RequestID = requestId,
                ReplyContent = replyContent,
                ReplyDate = DateTime.Now
            };

            // Pass 'ReplyContent' through the 'Reply' object
            // Ensure the 'Reply' object is added to the 'Replies' of the 'Request'
            request.Replies.Add(reply);
            db.SaveChanges();

            // Redirect to the specified action or controller after updating
            return RedirectToAction("Index", "Assignee"); // Chắc chắn rằng bạn đã đặt đúng tên của controller
        }

        // Check if the user has access to the request
        private bool CanUserAccessRequest(Request request)
        {
            // Add authentication logic here (e.g., based on current user and permissions)
            // Return true if the user has access, otherwise return false
            return true;
        }

        // Check if the user has the right to change the status
        private bool CanUserChangeStatus(Request request)
        {
            // Add permission checking logic here (e.g., check if the user is the request manager)
            // Return true if the user has the right, otherwise return false
            return true;
        }

        public ActionResult RequirementHistory()
        {
            int accountID = (int)Session["AccountID"];

            if (Session["AccountID"] != null)
            {
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }

            int faciID = (int)Session["FaciID"];
            string query = @"
    SELECT 
        R.RequestID,
        R.RequestorID,
        RA.FullName AS RequestorFullName,
        F.FacilityName,
        R.RequestDate,
        R.AssigneeID,
        RL.ReplyContent,
        AA.FullName AS AssigneeFullName,
        R.Status,
        R.FacilityID,
        R.RequestContent,
        R.SeverityLevels
    FROM Requests R
    INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
    INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
    INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
    INNER JOIN Reply RL ON R.RequestID = RL.RequestID
    WHERE R.AssigneeID = @accountID and R.FacilityID = @faciID AND R.Status ='Completed'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@faciID", faciID);
                command.Parameters.AddWithValue("@accountID", accountID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    model.Add(new RequestHistory
                    {
                        RequestID = (int)reader["RequestID"],
                        RequestorID = (int)reader["RequestorID"],
                        RequestorFullName = reader["RequestorFullName"].ToString(),
                        FacilityName = reader["FacilityName"].ToString(),
                        RequestDate = (DateTime)reader["RequestDate"],
                        AssigneeID = (int)reader["AssigneeID"],
                        AssigneeFullName = reader["AssigneeFullName"].ToString(),
                        Status = reader["Status"].ToString(),
                        RequestContent = reader["RequestContent"].ToString(),
                        SeverityLevels = reader["SeverityLevels"].ToString(),
                        ReplyContent = reader["ReplyContent"].ToString(),
                    });
                }
            }
            int faciID1 = (int)Session["FaciID"];
            var assignees = db.Accounts.Where(u => u.TypeID == 4 && u.FacilityID == faciID1).ToList();

            ViewBag.Assignees = assignees;

            return View(model);
        }
        // AssigneesController.cs
        private List<Request> GetRequestsFromDatabase()
        {
            // Implement your logic to fetch requests from the database
            // Example: Replace this with your actual database retrieval code
            var requests = db.Requests.ToList();

            return requests;
        }
        private Dictionary<string, int> CalculateRatios(List<SummaryGroup> model)
        {
            Dictionary<string, int> ratios = new Dictionary<string, int>();

            // Calculate ratios based on total requests
            int totalRequests = model.Sum(s => s.TotalRequests);
            ratios["TotalRequests"] = totalRequests;

            // Calculate ratios for each severity level
            int totalLowRequests = model.Sum(s => s.LowRequests);
            int totalMediumRequests = model.Sum(s => s.MediumRequests);
            int totalHeavyRequests = model.Sum(s => s.HeavyRequests);

            ratios["LowRequests"] = totalLowRequests;
            ratios["MediumRequests"] = totalMediumRequests;
            ratios["HeavyRequests"] = totalHeavyRequests;

            return ratios;
        }

        private List<SummaryGroup> GetDataForSummaryReport()
        {
            List<Request> requests = GetRequestsFromDatabase();

            var groupedRequests = requests.GroupBy(r => r.Status)
                .OrderBy(group => group.Key)
                .Select(group => new SummaryGroup
                {
                    TotalRequests = group.Count(),
                    LowRequests = group.Count(r => r.SeverityLevels == "Low"),
                    MediumRequests = group.Count(r => r.SeverityLevels == "Medium"),
                    HeavyRequests = group.Count(r => r.SeverityLevels == "Heavy"),
                    Reports = group.SelectMany(r => r.SummaryReports.Select(sr => sr.Report))
                        .OrderBy(report => report.ReportMonth)
                        .ToList()
                })
                .OrderBy(group => group.Reports.FirstOrDefault()?.ReportMonth)
                .ToList();

            return groupedRequests;
        }




        public ActionResult CreateSummaryReport()
        {
            // Get data for summary report
            List<SummaryGroup> model = GetDataForSummaryReport();

            // Calculate ratios based on total requests
            Dictionary<string, int> ratios = CalculateRatios(model);

            // Set the total requests to ViewData for use in the view
            ViewData["TotalRequests"] = ratios["TotalRequests"];

            // Add ratios to ViewData for each severity level (Low, Medium, Heavy)
            ViewData["LowRequests"] = ratios.ContainsKey("LowRequests") ? ratios["LowRequests"] : 0;
            ViewData["MediumRequests"] = ratios.ContainsKey("MediumRequests") ? ratios["MediumRequests"] : 0;
            ViewData["HeavyRequests"] = ratios.ContainsKey("HeavyRequests") ? ratios["HeavyRequests"] : 0;

            ViewData["Reports"] = model;

            // Return the list of requests to the view
            return View(model);
        }

        public ActionResult CreateReport()
        {
            int accountID = (int)Session["AccountID"];

            if (Session["AccountID"] != null)
            {

                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            ViewBag.RequestDate = db.Requests.ToList();
            ViewBag.ReportDate = db.Reports.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReport(SummaryReport sum)
        {
            int accountID = (int)Session["AccountID"];

            if (Session["AccountID"] != null)
            {

                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            ViewBag.RequestDate = db.Requests.ToList();
            ViewBag.ReportDate = db.Reports.ToList();
            return View("CreateReport");
        }



        private List<string> GetStatuses()
        {
            // Function to retrieve a list of statuses (needs implementation)
            return new List<string> { "Assigned", "In Progress", "Completed", /* Add other statuses as needed */ };
        }

        //Change Account Information
        public ActionResult Information()
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var accountID1 = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (accountID1 != null)
                {
                    int userID = accountID1.AccountID;
                    ViewBag.FullName = accountID1.Fullname;
                    ViewBag.Birthday = ((DateTime)accountID1.Birthday).ToString("yyyy-MM-dd");
                    Account acc = db.Accounts.Where(r => r.AccountID.Equals(userID)).FirstOrDefault();
                    return View(acc);
                }
            }
            return RedirectToAction("Index", "Assignee");
        }

        [HttpPost, ActionName("Information")]
        public ActionResult ChangeInformation(string FullName, string Gender, string Phone, DateTime Birthday)
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var accountID1 = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (accountID1 != null)
                {
                    int userID = accountID1.AccountID;
                    ViewBag.FullName = accountID1.Fullname;
                    ViewBag.Birthday = ((DateTime)accountID1.Birthday).ToString("yyyy-MM-dd");
                    using (var context = new OHDDBContext())
                    {
                        var user = context.Accounts.SingleOrDefault(u => u.AccountID == userID);

                        if (user != null)
                        {
                            user.Fullname = FullName;
                            user.Gender = Gender;
                            user.Phone = Phone;
                            user.Birthday = (DateTime)Birthday;
                            context.SaveChanges();
                            ViewBag.Successfully = "Change information successfully!";

                            return RedirectToAction("Information", "Assignee");
                        }
                    }
                }
            }
            return RedirectToAction("Information", "Assignee");
        }

        //Change Password
        public ActionResult ChangePassword()
        {
            int accountID = (int)Session["AccountID"];
            var accountID1 = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

            int userID = accountID1.AccountID;
            ViewBag.FullName = accountID1.Fullname;
            Account acc = db.Accounts.Where(r => r.AccountID.Equals(userID)).FirstOrDefault();
            return View(acc);
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var accountID1 = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (accountID1 != null)
                {
                    int userID = accountID1.AccountID;
                    ViewBag.FullName = accountID1.Fullname;

                    if (confirmPassword.Equals(newPassword))
                    {
                        if (ChangePasswordLogic(oldPassword, newPassword, userID))
                        {
                            ViewBag.Message = "Changed password successfully.";
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Old password is incorrect.";
                        }
                    }
                    else
                    {
                        ViewBag.Confirmation = "Confirm password does not match.";
                    }
                }
            }
            return View();
        }

        private string GetCurrentPasswordFromDatabase(int userID)
        {
            using (var db = new OHDDBContext())
            {
                var user = db.Accounts.SingleOrDefault(u => u.AccountID == userID);

                if (user != null)
                {
                    return user.Password;
                }

                return null;
            }
        }

        private bool VerifyOldPassword(string oldPassword, int userID)
        {
            string currentPasswordHash = GetCurrentPasswordFromDatabase(userID);

            if (currentPasswordHash != null)
            {
                return VerifyPassword(oldPassword, currentPasswordHash);
            }

            return false;
        }

        private bool ChangePasswordLogic(string oldPassword, string newPassword, int userID)
        {
            if (VerifyOldPassword(oldPassword, userID))
            {
                using (var context = new OHDDBContext())
                {
                    var user = context.Accounts.SingleOrDefault(u => u.AccountID == userID);

                    if (user != null)
                    {
                        user.Password = HashPassword(newPassword);
                        context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string enteredPasswordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return enteredPasswordHash == hashedPassword;
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}