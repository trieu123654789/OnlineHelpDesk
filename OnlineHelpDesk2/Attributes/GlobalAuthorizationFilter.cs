using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineHelpDesk2.Models;

namespace OnlineHelpDesk2.Attributes
{
    /// <summary>
    /// Global filter to handle session validation and prevent access to restricted areas
    /// </summary>
    public class GlobalAuthorizationFilter : ActionFilterAttribute
    {
        private readonly string[] _publicControllers = { "WelcomePage", "Home" };
        private readonly string[] _publicActions = { "Login", "Index", "GuestLetter", "ForgotPassword", "Logout" };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"]?.ToString();
            var action = filterContext.RouteData.Values["action"]?.ToString();

            // Skip authorization for public controllers and actions
            if (IsPublicAccess(controller, action))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

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

            // Validate session integrity - ensure user still exists in database
            try
            {
                int accountId = (int)httpContext.Session["AccountID"];
                using (var db = new OHDDBContext())
                {
                    var user = db.Accounts.FirstOrDefault(u => u.AccountID == accountId);
                    
                    if (user == null)
                    {
                        // User no longer exists, clear session and redirect to login
                        httpContext.Session.Clear();
                        filterContext.Result = new RedirectToRouteResult(
                            new System.Web.Routing.RouteValueDictionary
                            {
                                {"controller", "WelcomePage"},
                                {"action", "Login"}
                            });
                        return;
                    }

                    // Additional security check: Ensure user is accessing appropriate controller
                    if (!IsValidControllerAccess(controller, user.TypeID))
                    {
                        // Redirect to appropriate home page
                        var redirectRoute = GetHomeRouteForUser(user.TypeID);
                        filterContext.Result = new RedirectToRouteResult(redirectRoute);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                // In case of any error, clear session and redirect to login
                httpContext.Session.Clear();
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        {"controller", "WelcomePage"},
                        {"action", "Login"}
                    });
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsPublicAccess(string controller, string action)
        {
            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
                return false;

            return _publicControllers.Contains(controller, StringComparer.OrdinalIgnoreCase) ||
                   _publicActions.Contains(action, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsValidControllerAccess(string controller, int userTypeId)
        {
            if (string.IsNullOrEmpty(controller))
                return false;

            switch (controller.ToLower())
            {
                case "admin":
                    return userTypeId == 1;
                case "enduser":
                    return userTypeId == 2;
                case "faciheader":
                    return userTypeId == 3;
                case "assignee":
                    return userTypeId == 4;
                case "home":
                case "welcomepage":
                    return true; // These are generally accessible
                default:
                    return false;
            }
        }

        private System.Web.Routing.RouteValueDictionary GetHomeRouteForUser(int userTypeId)
        {
            switch (userTypeId)
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
}
