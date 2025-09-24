using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OnlineHelpDesk2.Models;
using static System.Collections.Specialized.BitVector32;

namespace OnlineHelpDesk2.Controllers
{
    [FacilityHeaderOnly]
    public class FaciHeaderController : Controller
    {
        // GET: FaciHeader
        private OHDDBContext db = new OHDDBContext();
        public ActionResult Index()
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            int faciID = (int)Session["FaciID"];
            string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
            string query = @"
            select  R.RequestID,
        R.RequestorID,
        RA.FullName AS RequestorFullName,
        F.FacilityName,
        R.RequestDate,
R.FacilityID,
        R.Status,
        R.RequestContent,
        R.SeverityLevels
    FROM Requests R
    INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
    INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
    WHERE R.FacilityID = @FacilityID AND R.Status='Unassigned'";

            List<UnassignedRequest> model = new List<UnassignedRequest>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FacilityID", faciID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    model.Add(new UnassignedRequest
                    {
                        RequestID = (int)reader["RequestID"],
                        RequestorID = (int)reader["RequestorID"],
                        RequestorFullName = reader["RequestorFullName"].ToString(),
                        FacilityName = reader["FacilityName"].ToString(),
                        RequestDate = (DateTime)reader["RequestDate"],
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

        public ActionResult AssignRequest(int requestId, int assigneeId)
        {

            Request request = db.Requests.FirstOrDefault(r => r.RequestID == requestId);

            return PartialView("Assign", request);
        }


        public ActionResult RequestList()
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            int faciID = (int)Session["FaciID"];
            string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
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
        R.RequestContent,
        R.SeverityLevels
    FROM Requests R
    INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
    INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
    INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
    WHERE R.FacilityID = @FacilityID AND R.Status ='In Progress'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FacilityID", faciID);
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
        public ActionResult Assign(int id)
        {
            Request req = db.Requests.Where(row => row.RequestID == id).FirstOrDefault();
            int faciID1 = (int)Session["FaciID"];
            var assignees = db.Accounts.Where(u => u.TypeID == 4 && u.FacilityID == faciID1).ToList();

            ViewBag.Assignees = assignees;
            return View(req);
        }

        [HttpPost]
        public ActionResult Assign(int RequestID, int AssigneeID)
        {

            Request request = db.Requests.Find(RequestID);

            if (request != null)
            {
                request.AssigneeID = AssigneeID;
                request.Status = "In Progress";

                db.SaveChanges();

                return RedirectToAction("Index", "FaciHeader");
            }
            else
            {
                return View();
            }
        }


        public ActionResult CloseRequest(int id)
        {

            Request req = db.Requests.Where(row => row.RequestID == id).FirstOrDefault();
            return View(req);
        }
        [HttpPost]

        public ActionResult CloseRequest(CloseRequest req)
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            if (ModelState.IsValid)
            {
                db.CloseRequests.Add(req);
                db.SaveChanges();
                Request request = db.Requests.Find(req.RequestID);
                if (request != null)
                {
                    request.Status = "Rejected";
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "FaciHeader");
            }
            return View(req);
        }
        public ActionResult DenyRequest()
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            int faciID = (int)Session["FaciID"];
            string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
            string query = @"SELECT 
                    R.RequestID,
                    R.RequestorID,
                    RA.FullName AS RequestorFullName,
                    R.RequestDate,
                    R.Status,
                    R.RequestContent,
                    R.SeverityLevels,
	                C.Reason
                    FROM Requests R
                    INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                    INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                    INNER JOIN CloseRequest C ON R.RequestID=C.RequestID 
                    WHERE R.FacilityID=@FacilityID and Status = 'Rejected'";

            List<DenyRequest> model = new List<DenyRequest>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FacilityID", faciID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    model.Add(new DenyRequest
                    {
                        RequestID = Convert.ToInt32(reader["RequestID"]),
                        RequestorID = Convert.ToInt32(reader["RequestorID"]),
                        RequestorFullName = reader["RequestorFullName"].ToString(),
                        RequestDate = Convert.ToDateTime(reader["RequestDate"]),
                        Status = reader["Status"].ToString(),
                        RequestContent = reader["RequestContent"].ToString(),
                        SeverityLevels = reader["SeverityLevels"].ToString(),
                        Reason = reader["Reason"].ToString()
                    });
                }
            }
            return View(model);
        }

        public ActionResult Report()
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

        [HttpPost, ActionName("Report")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyReport(SummaryReport sum)
        {
            return View("CreateReport");
        }

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
            return RedirectToAction("Index", "FaciHeader");
        }

        [HttpPost]
        public ActionResult Information(string FullName, string Gender, string Phone, DateTime Birthday)
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
                            return RedirectToAction("Information");
                        }
                    }
                }
            }
            return RedirectToAction("Information");
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
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
        public ActionResult CompletedRequests()
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                }
            }
            int faciID = (int)Session["FaciID"];
            string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
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
        R.RequestContent,
        R.SeverityLevels
    FROM Requests R
    INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
    INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
    INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
    WHERE R.FacilityID = @FacilityID AND R.Status ='Completed'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FacilityID", faciID);
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


    }

}

