using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk2.Controllers
{
    /// <summary>
    /// Base controller class that provides common functionality for all controllers
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected OHDDBContext db;

        public BaseController()
        {
            db = new OHDDBContext();
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            // Set common ViewBag properties that are used across all controllers
            SetUserFullName();
        }

        /// <summary>
        /// Sets the user's full name in ViewBag for display in views
        /// </summary>
        protected virtual void SetUserFullName()
        {
            if (Session["AccountID"] != null)
            {
                int accountID = (int)Session["AccountID"];
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountID);

                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                    ViewBag.UserRole = user.TypeID;
                    ViewBag.UserTypeName = user.UserType?.TypeName;
                }
            }
        }

        /// <summary>
        /// Gets the current user's account information
        /// </summary>
        /// <returns>Account object or null if user is not logged in</returns>
        protected Account GetCurrentUser()
        {
            if (Session["AccountID"] == null)
                return null;

            int accountID = (int)Session["AccountID"];
            return db.Accounts.FirstOrDefault(u => u.AccountID == accountID);
        }

        /// <summary>
        /// Checks if the current user has a specific role
        /// </summary>
        /// <param name="roleId">The role ID to check for</param>
        /// <returns>True if user has the role, false otherwise</returns>
        protected bool HasRole(int roleId)
        {
            var user = GetCurrentUser();
            return user != null && user.TypeID == roleId;
        }

        /// <summary>
        /// Checks if the current user has any of the specified roles
        /// </summary>
        /// <param name="roleIds">Array of role IDs to check for</param>
        /// <returns>True if user has any of the roles, false otherwise</returns>
        protected bool HasAnyRole(params int[] roleIds)
        {
            var user = GetCurrentUser();
            return user != null && roleIds.Contains(user.TypeID);
        }

        /// <summary>
        /// Redirects user to their appropriate home page based on their role
        /// </summary>
        /// <returns>RedirectToRouteResult to the appropriate controller/action</returns>
        protected ActionResult RedirectToUserHome()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "WelcomePage");
            }

            switch (user.TypeID)
            {
                case 1: // Admin
                    return RedirectToAction("Index", "Admin");
                case 2: // End User
                    return RedirectToAction("Index", "Enduser");
                case 3: // Facility Header
                    return RedirectToAction("Index", "FaciHeader");
                case 4: // Assignee
                    return RedirectToAction("Index", "Assignee");
                default:
                    return RedirectToAction("Login", "WelcomePage");
            }
        }

        /// <summary>
        /// Validates that the user is logged in and optionally has a specific role
        /// </summary>
        /// <param name="requiredRole">Optional role ID that the user must have</param>
        /// <returns>True if validation passes, false otherwise</returns>
        protected bool ValidateUserAccess(int? requiredRole = null)
        {
            var user = GetCurrentUser();
            if (user == null)
                return false;

            if (requiredRole.HasValue && user.TypeID != requiredRole.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Returns a JSON result indicating unauthorized access
        /// </summary>
        /// <returns>JSON result with error message</returns>
        protected JsonResult UnauthorizedJsonResult()
        {
            return Json(new { success = false, message = "Unauthorized access" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Clears the user session and redirects to login
        /// </summary>
        /// <returns>RedirectToRouteResult to login page</returns>
        protected ActionResult LogoutUser()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "WelcomePage");
        }

        /// <summary>
        /// Disposes the database context
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles errors that occur during action execution
        /// </summary>
        /// <param name="filterContext">The filter context</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            // Log the exception (you can implement your own logging mechanism)
            // For now, we'll just handle database-related errors that might indicate
            // session issues
            if (filterContext.Exception is System.Data.Entity.Core.EntityException ||
                filterContext.Exception is InvalidOperationException)
            {
                // Clear potentially corrupted session
                Session.Clear();
                filterContext.Result = RedirectToAction("Login", "WelcomePage");
                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }
    }
}
