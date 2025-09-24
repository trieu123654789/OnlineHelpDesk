using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk2
{
    /// <summary>
    /// Base class for role-based authorization attributes
    /// </summary>
    public abstract class RoleAuthorizationAttribute : ActionFilterAttribute
    {
        protected abstract int[] AllowedRoles { get; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            
            // Check if session exists and user is logged in
            if (httpContext.Session == null || httpContext.Session["AccountID"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
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
                        new RouteValueDictionary
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

        private RouteValueDictionary GetRedirectRouteForUserType(int typeId)
        {
            switch (typeId)
            {
                case 1: // Admin
                    return new RouteValueDictionary
                    {
                        {"controller", "Admin"},
                        {"action", "Index"}
                    };
                case 2: // End User
                    return new RouteValueDictionary
                    {
                        {"controller", "Enduser"},
                        {"action", "Index"}
                    };
                case 3: // Facility Header
                    return new RouteValueDictionary
                    {
                        {"controller", "FaciHeader"},
                        {"action", "Index"}
                    };
                case 4: // Assignee
                    return new RouteValueDictionary
                    {
                        {"controller", "Assignee"},
                        {"action", "Index"}
                    };
                default:
                    return new RouteValueDictionary
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
    }

    /// <summary>
    /// Restricts access to End Users only (TypeID = 2)
    /// </summary>
    public class EndUserOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 2 };
    }

    /// <summary>
    /// Restricts access to Facility Headers only (TypeID = 3)
    /// </summary>
    public class FacilityHeaderOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 3 };
    }

    /// <summary>
    /// Restricts access to Assignees only (TypeID = 4)
    /// </summary>
    public class AssigneeOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 4 };
    }

    /// <summary>
    /// Allows access to both Facility Headers and Assignees (TypeID = 3 or 4)
    /// </summary>
    public class FacilityStaffOnlyAttribute : RoleAuthorizationAttribute
    {
        protected override int[] AllowedRoles => new[] { 3, 4 };
    }
}
