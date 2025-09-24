using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk1.Controllers
{
    public class WelcomePageController : Controller
    {
        // GET: WelcomePage
        OHDDBContext db = new OHDDBContext();
        List<RequestHistory> requestHistories = new List<RequestHistory>();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
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
        public ActionResult Login(string username, string password)
        {
            var admin = db.Accounts.FirstOrDefault(u => u.Username == username && u.TypeID == 1);
            var enduser = db.Accounts.FirstOrDefault(u => u.Username == username && u.TypeID == 2);
            var facilityhead = db.Accounts.FirstOrDefault(u => u.Username == username && u.TypeID == 3);
            var assignee = db.Accounts.FirstOrDefault(u => u.Username == username && u.TypeID == 4);

            if (admin != null && VerifyPassword(password, admin.Password))
            {
                Session["AccountID"] = admin.AccountID;
                return RedirectToAction("Index", "Admin");
            }
            else if (enduser != null && VerifyPassword(password, enduser.Password))
            {
                Session["AccountID"] = enduser.AccountID;
                return RedirectToAction("Create", "Enduser");
            }
            else if (facilityhead != null && VerifyPassword(password, facilityhead.Password))
            {
                Session["AccountID"] = facilityhead.AccountID;
                Session["FaciID"] = facilityhead.FacilityID;
                return RedirectToAction("Index", "FaciHeader");
            }
            else if (assignee != null && VerifyPassword(password, assignee.Password))
            {
                Session["AccountID"] = assignee.AccountID;
                Session["FaciID"] = assignee.FacilityID;
                return RedirectToAction("Index", "Assignee");
            }
            else
            {
                ViewBag.LoginFail = "Username or password is incorrect";
                return View();
            }
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
                byte[] hashedBytes = sha256.ComputeHash(enteredPasswordBytes);
                string enteredPasswordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return enteredPasswordHash == hashedPassword;
            }
        }
        public ActionResult GuestLetter()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuestLetter([Bind(Include = "Name,Mail,LetterContent")] GuestLetter letter)
        {
            if (ModelState.IsValid)
            {
                db.GuestLetters.Add(letter);
                db.SaveChanges();
                ViewBag.SuccessMessage = "Request sent successfully!";
                return View(letter);

            }
            else
            {
                ViewBag.Notification = "Can not send letter!";
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public string RandomPass()
        {
            int Numrd;
            string Numrd_str;
            Random rd = new Random();
            Numrd_str = rd.Next(100000, 1000000).ToString();
            return Numrd_str;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            var user = db.Accounts.FirstOrDefault(u => u.Email == Email);
            if (user != null)
            {
                string newPass = RandomPass();
                user.Password = HashPassword(newPass);
                db.SaveChanges();

                var mail = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("ohdadm123654@gmail.com", "khfn zgqi lxlj iwgf"),
                    EnableSsl = true
                };

                var message = new MailMessage();
                message.From = new MailAddress("ohdadm123654@gmail.com");
                message.To.Add(new MailAddress(user.Email));
                message.Subject = "Reset OHD Password Account";
                string loginLink = "http://localhost:52093/WelcomePage/Login";
                message.Body = "Your new password is: " + newPass + ", please login at " + loginLink + " to change your password";

                mail.Send(message);

                ViewBag.Success1 = "Send mail successful! Please check your mail to see information";
                return View();
            }
            else
            {
                ViewBag.Alert = "Email is not exist";
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "WelcomePage");
        }

    }
}
