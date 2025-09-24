using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk2.Attributes
{
    /// <summary>
    /// Base class for role-based authorization attributes
    /// </summary>
    public abstract class RoleAuthorizationAttribute : ActionFilterAttribute
    {
        protected abstract int[] AllowedRoles { get; }
        protected abstract string RedirectController { get; }
        protected abstract string RedirectAction { get; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            
            // Check if session exists and user is logged in
            if (httpContext.Session == null || httpContext.Session["AccountID"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "WelcomePage"},
                        {"action", "Login"}
                    });
                return;
            }

            // Get user's account ID from session
            int accountId = (int)httpContext.Session["AccountID"];
            
            // Check user's role from database
            using (var db = new OHDDBContext())
            {
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountId);
                
                if (user == null)
                {
                    // User not found, redirect to login
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            {"controller", "WelcomePage"},
                            {"action", "Login"}
                        });
                    return;
                }

                // Check if user's role is allowed
                if (!AllowedRoles.Contains(user.TypeID))
                {
                    // User doesn't have permission, redirect to their appropriate page
                    var redirectRoute = GetRedirectRouteForUserType(user.TypeID);
                    filterContext.Result = new RedirectToRouteResult(redirectRoute);
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private System.Web.Routing.RouteValueDictionary GetRedirectRouteForUserType(int typeId)
        {
            switch (typeId)
            {
                case 1: // Admin
                    return new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Admin"},
                        {"action", "Index"}
                    };
                case 2: // End User
                    return new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Enduser"},
                        {"action", "Index"}
                    };
                case 3: // Facility Header
                    return new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "FaciHeader"},
                        {"action", "Index"}
                    };
                case 4: // Assignee
                    return new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "Assignee"},
                        {"action", "Index"}
                    };
                default:
                    return new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "WelcomePage"},
                        {"action", "Login"}
                    };
            }
        }
    }

    /// <summary>
    /// Restricts access to Admin users only (TypeID = 1)
    /// </summary>
    public class AdminOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 1 };
        protected override string RedirectController => "Admin";
        protected override string RedirectAction => "Index";
    }

    /// <summary>
    /// Restricts access to End Users only (TypeID = 2)
    /// </summary>
    public class EndUserOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 2 };
        protected override string RedirectController => "Enduser";
        protected override string RedirectAction => "Index";
    }

    /// <summary>
    /// Restricts access to Facility Headers only (TypeID = 3)
    /// </summary>
    public class FacilityHeaderOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 3 };
        protected override string RedirectController => "FaciHeader";
        protected override string RedirectAction => "Index";
    }

    /// <summary>
    /// Restricts access to Assignees only (TypeID = 4)
    /// </summary>
    public class AssigneeOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 4 };
        protected override string RedirectController => "Assignee";
        protected override string RedirectAction => "Index";
    }

    /// <summary>
    /// Allows access to both Facility Headers and Assignees (TypeID = 3 or 4)
    /// This is useful for actions that both roles should be able to access
    /// </summary>
    public class FacilityStaffOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 3, 4 };
        protected override string RedirectController => "WelcomePage";
        protected override string RedirectAction => "Login";
    }

    /// <summary>
    /// Requires user to be logged in but allows any role
    /// </summary>
    public class AuthenticatedOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            
            // Check if session exists and user is logged in
            if (httpContext.Session == null || httpContext.Session["AccountID"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "WelcomePage"},
                        {"action", "Login"}
                    });
                return;
            }

            // Verify the user still exists in database
            int accountId = (int)httpContext.Session["AccountID"];
            using (var db = new OHDDBContext())
            {
                var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountId);
                
                if (user == null)
                {
                    // User not found, clear session and redirect to login
                    httpContext.Session.Clear();
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary
                        {
                            {"controller", "WelcomePage"},
                            {"action", "Login"}
                        });
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
