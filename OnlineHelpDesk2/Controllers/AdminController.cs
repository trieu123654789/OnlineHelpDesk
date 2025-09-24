using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using OfficeOpenXml;
using System.Web.Mvc;
using OnlineHelpDesk2.Models;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Web.Helpers;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace OnlineHelpDesk2.Controllers
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session.IsNewSession)
            {
                string sessionCookie = filterContext.HttpContext.Request.Headers["Cookie"];

                if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                {
                    filterContext.Result = new RedirectResult("~/WelcomePage/Login");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
    
    [AdminOnly]
    public class AdminController : Controller
    {
        OHDDBContext db = new OHDDBContext();
        string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;

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
            List<Account> acc = db.Accounts.ToList();
            return View(acc);
        }

        [HttpGet]
        public ActionResult Index(string searchString)
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
            string connectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
            string query = @"SELECT A.AccountID, A.Username, A.Password, A.Email, U.TypeName, F.FacilityName, A.Fullname, A.Gender, A.Birthday, A.Phone FROM Accounts A INNER JOIN UserTypes U ON A.TypeID = U.TypeID INNER JOIN Facilities F ON A.FacilityID = F.FacilityID;";

            List<AccountList> model = new List<AccountList>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    model.Add(new AccountList
                    {
                        AccountID = (int)reader["AccountID"],
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        Email = reader["Email"].ToString(),
                        TypeName = reader["TypeName"].ToString(),
                        FacilityName = reader["FacilityName"].ToString(), 
                        Fullname = reader["Fullname"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Birthday = (DateTime)reader["Birthday"],
                        Phone = reader["Phone"].ToString()
                    });
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
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
            var userTypes = db.UserTypes.ToList();
            SelectList userTypeList = new SelectList(userTypes, "TypeID", "TypeName");
            ViewBag.UserTypeList = userTypeList;
            var facilities = new SelectList(db.Facilities, "FacilityID", "FacilityName");
            ViewBag.FacilityList = facilities;
            ViewBag.Gender = new SelectList(new[] { "Male", "Female" });
            var defaultAccount = new Account { Birthday = DateTime.Now.Date };
            return View(defaultAccount);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account viewModel)
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
            try
            {
                if (ModelState.IsValid)
                {
                    // Hash the password before saving to the database
                    string hashedPassword = HashPassword(viewModel.Password);

                    // Check if the username already exists
                    if (db.Accounts.Any(a => a.Username == viewModel.Username))
                    {
                        ModelState.AddModelError("Username", "The provided username is already in use.");
                    }
                    // Check if the email already exists
                    else if (db.Accounts.Any(a => a.Email == viewModel.Email))
                    {
                        ModelState.AddModelError("Email", "The provided email address is already in use.");
                    }
                    // Check if the phone already exists
                    else if (db.Accounts.Any(a => a.Phone == viewModel.Phone))
                    {
                        ModelState.AddModelError("Phone", "The provided phone is already in use.");
                    }
                    // Check if the phone number starts with 0 and has 10 digits
                    else if (!Regex.IsMatch(viewModel.Phone, @"^0[0-9]{9}$"))
                    {
                        ModelState.AddModelError("Phone", "Invalid Phone number, Phone number must start with 0 and have 10 digits.");
                    }
                    // Check if the password meets the criteria
                    else if (!Regex.IsMatch(viewModel.Password, @".{6,}"))
                    {
                        ModelState.AddModelError("Password", "Password must be at least 6 characters.");
                    }
                    // Check if the birthday indicates the student is at least 18 years old
                    else if (viewModel.Birthday > DateTime.Now.AddYears(-18))
                    {
                        ModelState.AddModelError("Birthday", "Students must be at least 18 years old.");
                    }
                    else
                    {
                        // Save the new account if both username and email are unique
                        var account = new Account
                        {
                            Username = viewModel.Username,
                            Password = hashedPassword,
                            Email = viewModel.Email,
                            TypeID = viewModel.TypeID,
                            FacilityID = viewModel.FacilityID,
                            Fullname = viewModel.Fullname,
                            Gender = viewModel.Gender,
                            Birthday = viewModel.Birthday,
                            Phone = viewModel.Phone
                        };

                        db.Accounts.Add(account);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the record.");
            }

            // If there's an error, update the ViewBag and return the view
            ViewBag.TypeID = new SelectList(db.UserTypes, "TypeID", "TypeName", viewModel.TypeID);
            ViewBag.FacilityID = new SelectList(db.Facilities, "FacilityID", "FacilityName", viewModel.FacilityID);
            ViewBag.Gender = new SelectList(new[] { "Male", "Female" }, viewModel.Gender);

            return View(viewModel);
        }



        public ActionResult CreateFromExcel()
        {

            //Lấy danh sách từ CSDL
            ViewBag.TypeID = new SelectList(db.UserTypes, "TypeID", "TypeName");
            ViewBag.FacilityID = new SelectList(db.Facilities, "FacilityID", "FacilityName");
            ViewBag.Gender = new SelectList(new[] { "Male", "Female" });

            // Khởi tạo một Account mới để truyền giá trị mặc định cho ngày sinh (tránh lỗi khi hiển thị DatePicker)
            var defaultAccount = new Account { Birthday = DateTime.Now.Date };
            return View(defaultAccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromExcel(HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {

                // Đường dẫn lưu trữ file Excel trên server
                string filePath = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(file.FileName));
                file.SaveAs(filePath);

                // Đọc dữ liệu từ file Excel
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet != null)
                    {
                        int rowCount = worksheet.Dimension.Rows;

                        // Kết nối đến cơ sở dữ liệu
                        string connectionString = "data source=ADMIN-PC;initial catalog=OnlineHelpDesk;user id=sa;password=sa;MultipleActiveResultSets=True;App=EntityFramework";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Thêm từng dòng từ file Excel vào cơ sở dữ liệu
                            for (int row = 2; row <= rowCount; row++)
                            {
                                // Kiểm tra giá trị null trước khi sử dụng
                                if (worksheet.Cells[row, 1].Value != null && worksheet.Cells[row, 2].Value != null)
                                {
                                    string username = worksheet.Cells[row, 1].Value.ToString();
                                    string password = worksheet.Cells[row, 2].Value.ToString();

                                    // Retrieve values for additional fields
                                    string email = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty;
                                    string userType = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty;
                                    string facility = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty;
                                    string fullname = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty;
                                    string gender = worksheet.Cells[row, 7].Value?.ToString() ?? string.Empty;
                                    string birthday = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty;
                                    string phone = worksheet.Cells[row, 9].Value?.ToString() ?? string.Empty;

                                    // Thực hiện thêm vào cơ sở dữ liệu (ví dụ sử dụng SqlCommand)
                                    string insertQuery = "INSERT INTO Accounts (Username, Password, Email, UserType, Facility, Fullname, Gender, Birthday, Phone) " +
                     "VALUES (@Username, @Password, @Email, @UserType, @Facility, @Fullname, @Gender, @Birthday, @Phone)";

                                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                                    {
                                        command.Parameters.AddWithValue("@Username", username);
                                        command.Parameters.AddWithValue("@Password", password);
                                        command.Parameters.AddWithValue("@Email", email);
                                        command.Parameters.AddWithValue("@UserType", userType);
                                        command.Parameters.AddWithValue("@Facility", facility);
                                        command.Parameters.AddWithValue("@Fullname", fullname);
                                        command.Parameters.AddWithValue("@Gender", gender);
                                        command.Parameters.AddWithValue("@Birthday", birthday);
                                        command.Parameters.AddWithValue("@Phone", phone);

                                        command.ExecuteNonQuery();
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Worksheet not found in Excel.";
                    }
                }

                // Xóa file Excel sau khi đọc xong
                System.IO.File.Delete(filePath);

                ViewBag.Message = "Import successfully!";
            }
            else
            {
                ViewBag.Message = "Please select an Excel file to import.";
            }

            return View();
        }

        private void ImportDataFromExcel(string filePath)
        {
            string conString = string.Empty;
            string extension = Path.GetExtension(filePath);

            switch (extension)
            {
                case ".xls":
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
                case ".xlsx":
                    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                    break;
            }
            DataTable dtExcel = ReadExcelFile(filePath, conString);
            string sqlBulkCopyConnectionString = ConfigurationManager.ConnectionStrings["OHDDBContext"].ConnectionString;
            BulkCopyToDatabase(dtExcel, "dbo.[Accounts]", sqlBulkCopyConnectionString);
        }

        private DataTable ReadExcelFile(string filePath, string conString)
        {
            DataTable dtExcel = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;
                        connExcel.Open();
                        DataTable dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * from [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dtExcel);
                        connExcel.Close();
                    }
                }
            }

            return dtExcel;
        }

        // Helper method để thực hiện SQL Bulk Copy vào cơ sở dữ liệu
        private void BulkCopyToDatabase(DataTable dt, string destinationTable, string connectionString)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    sqlBulkCopy.DestinationTableName = destinationTable;
                    // Thêm các ánh xạ cột cần thiết
                    sqlBulkCopy.ColumnMappings.Add("Username", "Username");
                    sqlBulkCopy.ColumnMappings.Add("Password", "Password");
                    sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                    sqlBulkCopy.ColumnMappings.Add("User Type", "UserType");
                    sqlBulkCopy.ColumnMappings.Add("Facility", "Facility");
                    sqlBulkCopy.ColumnMappings.Add("Fullname", "Fullname");
                    sqlBulkCopy.ColumnMappings.Add("Gender", "Gender");
                    sqlBulkCopy.ColumnMappings.Add("Birthday", "Birthday");
                    sqlBulkCopy.ColumnMappings.Add("Phone", "Phone");

                    con.Open();
                    sqlBulkCopy.WriteToServer(dt);
                    con.Close();
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);

            if (account == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserTypes = db.UserTypes.ToList();
            ViewBag.FacilityID = db.Facilities.ToList();
            ViewBag.Gender = new SelectList(new[] { "Male", "Female" }, account.Gender);
            return View(account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Account account)
        {
            var getID = db.Accounts.FirstOrDefault(a => a.AccountID == account.AccountID);
            ViewBag.UserTypes = db.UserTypes.ToList();
            ViewBag.FacilityID = db.Facilities.ToList();
            ViewBag.GenderList = new SelectList(new[] { "Male", "Female" }, account.Gender);
            ViewBag.Birthday = ((DateTime)getID.Birthday).ToString("yyyy-MM-dd");
            Account acc = db.Accounts.Where(a => a.AccountID == account.AccountID).FirstOrDefault();
            // Check if the username already exists (except for the current account being edited)
            if (db.Accounts.Any(a => a.Email == account.Email && a.AccountID != account.AccountID))
            {
                ModelState.AddModelError("Email", "The provided email address is already in use.");
            }
            else if (db.Accounts.Any(a => a.Phone == account.Phone && a.AccountID != account.AccountID))
            {
                ModelState.AddModelError("Phone", "The provided phone is already in use.");
            }
            else if (!Regex.IsMatch(account.Phone, @"^0[0-9]{9}$"))
            {
                ModelState.AddModelError("Phone", "Invalid Phone number, Phone number must start with 0 and have 10 digits.");
            }
            else if (account.Birthday > DateTime.Now.AddYears(-18))
            {
                ModelState.AddModelError("Birthday", "Students must be at least 18 years old.");
            }
            else
            {
                acc.Email = account.Email;
                acc.Fullname = account.Fullname;
                acc.Gender = account.Gender;
                acc.Phone = account.Phone;
                acc.Birthday = account.Birthday;
                acc.TypeID = account.TypeID;
                acc.FacilityID = account.FacilityID;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Account edited successfully.";
                return RedirectToAction("Index");
            }
            return View(account);
        }

        //Delete
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            var userTypes = db.UserTypes.ToList();
            SelectList userTypeList = new SelectList(userTypes, "TypeID", "TypeName");
            ViewBag.UserTypeList = userTypeList;
            var facilities = new SelectList(db.Facilities, "FacilityID", "FacilityName");
            ViewBag.FacilityList = facilities;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            return View(account);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConformed(int id)
        {
            try
            {
                //Remove the account
                Account account = db.Accounts.Find(id);
                db.Accounts.Remove(account);
                db.SaveChanges();

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while deleting the record.");
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult Facilities()
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
            List<Facility> facilities = db.Facilities.ToList();
            return View(facilities);
        }

        //Create
        [HttpGet]
        public ActionResult CreateFacility()
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFacility([Bind(Include = "FacilityName")] Facility facility)
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
            try
            {
                if (ModelState.IsValid)
                {
                    //Check if the facilityName already exists
                    if (db.Facilities.Any(f => f.FacilityName == facility.FacilityName))
                    {
                        ModelState.AddModelError("FacilityName", "The provided facility name is already in use");
                    }
                    else
                    {
                        db.Facilities.Add(facility);
                        db.SaveChanges();
                        return RedirectToAction("Facilities");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the record.");
            }
            return View(facility);
        }

        //Edit facilities
        [HttpGet]
        public ActionResult EditFacility(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facility facility = db.Facilities.Find(id);

            if (facility == null)
            {
                return HttpNotFound();
            }

            ViewBag.Facility = db.Facilities.ToList();
            return View(facility);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFacility([Bind(Include = "FacilityID, FacilityName ")] Facility facility)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Check if the facilityName already exists
                    if (db.Facilities.Any(f => f.FacilityName == facility.FacilityName))
                    {
                        ModelState.AddModelError("FacilityName", "The provided facility name is already in use");
                    }
                    else
                    {
                        db.Entry(facility).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["SuccessMessage"] = "Facility edited successfully.";
                    }

                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving the record.");
            }
            ViewBag.Facility = db.Facilities.ToList();
            return View(facility);
        }

        //Delete
        [HttpGet]
        public ActionResult DeleteFacility(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Facility facility = db.Facilities.Find(id);

            if (facility == null)
            {
                return HttpNotFound();
            }

            return View(facility);
        }
        [HttpPost, ActionName("DeleteFacility")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // Find and delete associated accounts
                var associatedAccounts = db.Accounts.Where(a => a.FacilityID == id).ToList();

                foreach (var account in associatedAccounts)
                {
                    db.Accounts.Remove(account);
                }

                // Remove the facility
                Facility facility = db.Facilities.Find(id);
                db.Facilities.Remove(facility);

                // Save changes
                db.SaveChanges();

                TempData["SuccessMessage"] = "Facility and associated accounts deleted successfully.";
                return RedirectToAction("Facilities", "Admin");
            }
            catch (Exception ex)
            {
                // Log or output the exception details for debugging
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while deleting the record.");
                return RedirectToAction("Facilities", "Admin"); // Redirect to the index page or another appropriate action
            }
        }

        public ActionResult UnassignedRequest()
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
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID WHERE R.Status = 'Unassigned' or R.Status = 'Rejected' or R.Status = 'Closed'";

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

        public ActionResult Requests()
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
                            INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID WHERE R.Status = 'Completed' OR R.Status = 'In Progress'";

            List<RequestHistory> model = new List<RequestHistory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
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

        public ActionResult GuestRequest()
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
            List<GuestLetter> guest = db.GuestLetters.ToList();
            return View(guest);
        }

        [HttpGet]
        public ActionResult DeleteGuestRequest(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GuestLetter guest = db.GuestLetters.Find(id);
            return View(guest);
        }
        [HttpPost, ActionName("DeleteGuestRequest")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConformedGuestRequest(int id)
        {
            try
            {
                GuestLetter guest = db.GuestLetters.Find(id);
                db.GuestLetters.Remove(guest);
                db.SaveChanges();

                return RedirectToAction("GuestRequest", "Admin");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while deleting the record.");
                return RedirectToAction("GuestRequest");
            }
        }

        public ActionResult Reply()
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
                            INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID";
            List<ReplyOfAssignee> model = new List<ReplyOfAssignee>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
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

        public ActionResult Feedback()
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
                            R.RequestDate, R.RequestContent, R.Status, R.SeverityLevels, R.AssigneeID, AA.FullName AS AssigneeFullName, FB.FeedbackID, FB.FeedbackTime, FB.FeedbackContent 
                            FROM Feedback FB INNER JOIN Requests R ON FB.RequestID = R.RequestID 
							INNER JOIN Accounts RA ON R.RequestorID = RA.AccountID
                            INNER JOIN Facilities F ON R.FacilityID = F.FacilityID
                            INNER JOIN Accounts AA ON R.AssigneeID = AA.AccountID";
            List<FeedbackList> model = new List<FeedbackList>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    model.Add(new FeedbackList
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
                        FeedbackID = (int)reader["FeedbackID"],
                        FeedbackTime = (DateTime)reader["FeedbackTime"],
                        FeedbackContent = reader["FeedbackContent"].ToString()
                    });
                }
            }
            return View(model);
        }
        public ActionResult SummaryReport()
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

        [HttpPost, ActionName("SummaryReport")]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyReport(SummaryReport sum)
        {
            return View("CreateReport");
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
            return RedirectToAction("Index", "Admin");
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

                            return RedirectToAction("Information", "Admin");
                        }
                    }
                }
            }
            return RedirectToAction("Information", "Admin");
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

    }

}
