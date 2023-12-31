using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk1.Controllers
{
    public class HomeController : Controller
    {
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

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {

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
        public ActionResult Logout()
        {
            Session.Clear(); 
            return RedirectToAction("Login", "WelcomePage");
        }
    }
}