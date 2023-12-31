using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk1.Controllers
{
    public class WelcomePageController : Controller
    {
        // GET: WelcomePage
        private OHDDBContext db = new OHDDBContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var admin = db.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password && u.TypeID == 1);
            var enduser = db.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password && u.TypeID == 2);
            var facilityhead = db.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password && u.TypeID == 3);
            var assignee = db.Accounts.FirstOrDefault(u => u.Username == username && u.Password == password && u.TypeID == 4);

            if (admin != null)
            {
                Session["AccountID"] = admin.AccountID;
                return RedirectToAction("Index", "Home");
            }
            else if (enduser != null)
            {
                Session["AccountID"] = enduser.AccountID;
                return RedirectToAction("Index", "Facility");
            }
            else if (facilityhead != null)
            {
                Session["AccountID"] = facilityhead.AccountID;
                return RedirectToAction("GuestLetter", "Guest");
            }
            else if (assignee != null)
            {
                Session["AccountID"] = assignee.AccountID;
                return RedirectToAction("About", "Home");
            }
            else
            {
                ViewBag.LoginFail = "Username or password is incorrect";
                return View();
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
                ViewBag.SuccessMessage = "Letter sent successfully!";
                return View(letter);

            }
            else
            {
                ViewBag.Notification = "Can not send letter!";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}