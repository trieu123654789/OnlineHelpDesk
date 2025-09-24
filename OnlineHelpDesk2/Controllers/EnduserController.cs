using OnlineHelpDesk2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineHelpDesk2.Controllers
{
    [EndUserOnly]
    public class EnduserController : Controller
    {
        // GET: Requests
        OHDDBContext db = new OHDDBContext();
        string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
        //Unassigned Requests
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
            int requestorID = (int)Session["AccountID"];
            string query = @"SELECT R.RequestID, R.RequestorID, RA.FullName AS RequestorFullName,F.FacilityName,
                            R.RequestDate, R.Status, R.RequestContent, R.SeverityLevels
                            FROM Requests R INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            WHERE RA.AccountID = @AccountID and R.Status = 'Unassigned'";
            List<UnassignedRequest> model = new List<UnassignedRequest>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", requestorID);
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
            return View(model);
        }
        //Closed Requests
        public ActionResult CLosedRequests()
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
            int requestorID = (int)Session["AccountID"];
            //Change status
            string query = @"SELECT R.RequestID, R.RequestorID, RA.FullName AS RequestorFullName,F.FacilityName,
                            R.RequestDate, R.Status, R.RequestContent, R.SeverityLevels
                            FROM Requests R INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            WHERE RA.AccountID = @AccountID and R.Status = 'Closed'";
            List<UnassignedRequest> model = new List<UnassignedRequest>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", requestorID);
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
            return View(model);
        }
        //Rejected Requests
        public ActionResult RejectedRequests()
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
            int requestorID = (int)Session["AccountID"];
            //Change status
            string query = @"SELECT R.RequestID, R.RequestorID, RA.FullName AS RequestorFullName,F.FacilityName,
                            R.RequestDate, R.Status, R.RequestContent, R.SeverityLevels
                            FROM Requests R INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            WHERE RA.AccountID = @AccountID and R.Status = 'Rejected'";
            List<UnassignedRequest> model = new List<UnassignedRequest>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", requestorID);
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
            return View(model);
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
                    request.Status = "Closed";
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Enduser");
            }
            return View(req);
        }

        //Show Completed Requests
        public ActionResult RequestHistory()
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
            int requestorID = (int)Session["AccountID"];
            string query = @"SELECT R.RequestID, R.RequestorID, RA.FullName AS RequestorFullName, F.FacilityName,
                            R.RequestDate, R.AssigneeID, AA.FullName AS AssigneeFullName,
                            R.Status, R.RequestContent, R.SeverityLevels
                            FROM Requests R
                            INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
                            WHERE RA.AccountID = @AccountID and R.Status = 'Completed'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", requestorID);
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
            return View(model);
        }
        //Show In Progrees Requests
        public ActionResult InProgressRequests()
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
            int requestorID = (int)Session["AccountID"];
            string query = @"SELECT R.RequestID, R.RequestorID, RA.FullName AS RequestorFullName, F.FacilityName,
                            R.RequestDate, R.AssigneeID, AA.FullName AS AssigneeFullName,
                            R.Status, R.RequestContent, R.SeverityLevels
                            FROM Requests R
                            INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
                            WHERE RA.AccountID = @AccountID and R.Status = 'In Progress'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", requestorID);
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
            return View(model);
        }
        public ActionResult Feedback(int id)
        {
            Request req = db.Requests.Where(row => row.RequestID == id).FirstOrDefault();
            return View(req);
        }
        [HttpPost]
        public ActionResult Feedback(Feedback feedback)
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
                db.Feedbacks.Add(feedback);
                db.SaveChanges();

                return RedirectToAction("RequestHistory", "Enduser");
            }

            return View(feedback);
        }

        // GET: Requests/Details/5
        public ActionResult Replies()
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
            int requestorID = (int)Session["AccountID"];
            string query = @"SELECT R.RequestID, R.RequestorID, RA.FullName AS RequestorFullName, F.FacilityName,
                            R.RequestDate, R.RequestContent, R.Status, R.SeverityLevels, R.AssigneeID, AA.FullName AS AssigneeFullName, RP.ReplyID, RP.ReplyDate, RP.ReplyContent 
                            FROM Reply RP INNER JOIN Requests R ON RP.RequestID = R.RequestID 
							INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID 
                            WHERE R.RequestorID = @AccountID";
            List<ReplyOfAssignee> model = new List<ReplyOfAssignee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AccountID", requestorID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    model.Add(new ReplyOfAssignee
                    {
                        RequestID = (int)reader["RequestID"],
                        RequestorID = (int)reader["RequestorID"],
                        RequestorFullName = reader["RequestorFullName"].ToString(),
                        FacilityName = reader["FacilityName"].ToString(),
                        RequestDate = (DateTime)reader["RequestDate"],
                        RequestContent = reader["RequestContent"].ToString(),
                        Status = reader["Status"].ToString(),
                        SeverityLevels = reader["SeverityLevels"].ToString(),
                        AssigneeID = (int)reader["AssigneeID"],
                        AssigneeFullName = reader["AssigneeFullName"].ToString(),
                        ReplyID = (int)reader["ReplyID"],
                        ReplyDate = (DateTime)reader["ReplyDate"],
                        ReplyContent = reader["ReplyContent"].ToString()
                    });
                }
            }
            return View(model);
        }

        // GET: Requests/Create
        public ActionResult Create()
        {
            ViewBag.FacilityName = db.Facilities.Where(row => row.FacilityID != 1).ToList();
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var accountID1 = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (accountID1 != null)
                {
                    ViewBag.FullName = accountID1.Fullname;
                }
            }
            return View();
        }

        // POST: Request/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int FacilityID, string SeverityLevels = "", string RequestContent = "")
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var accountID1 = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (accountID1 != null)
                {
                    int requestorID = accountID1.AccountID;

                    if (requestorID != 0 && FacilityID != 0 && !string.IsNullOrEmpty(SeverityLevels) && !string.IsNullOrEmpty(RequestContent))
                    {
                        string sqlCommand = "INSERT INTO Requests(RequestorID, FacilityID, RequestDate, Status, RequestContent, SeverityLevels) " +
                            "VALUES (@requestorID, @facilityID, @requestDate, @status, @requestContent, @severityLevels)";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            using (SqlCommand command = new SqlCommand(sqlCommand, connection))
                            {
                                command.Parameters.AddWithValue("@requestorID", requestorID);
                                command.Parameters.AddWithValue("@facilityID", FacilityID);
                                command.Parameters.AddWithValue("@requestDate", DateTime.Now);
                                command.Parameters.AddWithValue("@status", "Unassigned");
                                command.Parameters.AddWithValue("@requestContent", RequestContent);
                                command.Parameters.AddWithValue("@severityLevels", SeverityLevels);

                                command.ExecuteNonQuery();
                            }
                        }
                        return RedirectToAction("Index", "Enduser");
                    }
                    else
                    {
                        ViewBag.Error = "Description is required.";
                    }
                }
            }
            return View();
        }
        // GET: Requests/Delete/5
        public ActionResult Delete(int id)
        {
            Request request = db.Requests.Where(r => r.RequestID == id).FirstOrDefault();
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id, Request request)
        {
            request = db.Requests.Where(row => row.RequestID.Equals(id)).FirstOrDefault();
            if (request == null)
            {
                return HttpNotFound();
            }
            db.CloseRequests.RemoveRange(request.CloseRequests);
            db.Requests.Remove(request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ViewReason(int id)
        {
            var request = db.Requests.Where(r => r.RequestID == id).FirstOrDefault();
            var reason = db.CloseRequests.Where(r => r.RequestID == id).FirstOrDefault();
            if (reason != null)
            {
                ViewBag.ReasonDescription = reason.Reason;
            }
            else
            {
                ViewBag.ReasonDescription = "No reason available.";
            }
            return View(request);
        }

        public ActionResult ViewFeedback(int id)
        {
            var request = db.Requests.Where(r => r.RequestID == id).FirstOrDefault();
            var feedback = db.Feedbacks.Where(r => r.RequestID == id).FirstOrDefault();
            if (feedback != null)
            {
                ViewBag.FeedbackContent = feedback.FeedbackContent;
                ViewBag.FeedbackTime = feedback.FeedbackTime;
                ViewBag.FeedbackID = feedback.FeedbackID;
            }
            else
            {
                ViewBag.FeedbackContent = "No reason available.";
            }
            return View(request);
        }

        public ActionResult EditFeedback(int id)
        {

            Feedback fb = db.Feedbacks.Where(row => row.FeedbackID == id).FirstOrDefault();
            return View(fb);
        }
        [HttpPost]

        public ActionResult EditFeedback(Feedback feedback)
        {
            Feedback fb = db.Feedbacks.Where(row => row.FeedbackID == feedback.FeedbackID).FirstOrDefault();
            //Update
            fb.FeedbackContent = feedback.FeedbackContent;
            fb.FeedbackTime = feedback.FeedbackTime;

            db.SaveChanges();
            return RedirectToAction("RequestHistory");
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
            return RedirectToAction("Index", "Enduser");
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

                            return RedirectToAction("Information", "Enduser");
                        }
                    }
                }
            }
            return RedirectToAction("Information", "Enduser");
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
