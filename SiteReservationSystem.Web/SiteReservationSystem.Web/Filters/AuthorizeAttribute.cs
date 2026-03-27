using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SiteReservationSystem.Web.Models;

// Handles authorization for all controllers - no longer have to call BaseController.RequireAdmin()/Permission() for each function
namespace SiteReservationSystem.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public AccessPermissions RequiredPermission { get; }

        public AuthorizeAttribute(AccessPermissions permission = AccessPermissions.None)
        {
            RequiredPermission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;

            var userId = session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var userRole = session.GetString("UserRole");
            if (userRole == "Admin")
            {
                return;
            }

            if (RequiredPermission != AccessPermissions.None)
            {
                var permissions = session.GetInt32("Permissions");
                if (permissions == null || ((AccessPermissions)permissions & RequiredPermission) != RequiredPermission)
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }
            else if (userRole != "Admin")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}
