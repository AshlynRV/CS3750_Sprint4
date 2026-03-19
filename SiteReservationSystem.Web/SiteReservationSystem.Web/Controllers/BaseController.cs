using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SiteReservationSystem.Web.Models;

public class BaseController : Controller
{
    // Call this at the top of any action that requires login
    // Check if user is logged in by checking if HttpContext.Session.GetInt32("UserID") is not null, this was set in AccountController.Login
    protected bool IsLoggedIn()
    {
        return HttpContext.Session.GetInt32("UserID").HasValue;
    }

    // Check if user is an admin by checking if HttpContext.Session.GetString("UserRole") is "Admin"
    // This is set in AccountController.Login
    protected bool IsAdmin()
    {
        return HttpContext.Session.GetString("UserRole") == "Admin";
    }

    // Returns true if user has permission, otherwise returns false
    // Check if user has permission to access a page based off HttpContext.Session.GetInt32("Permissions")
    // This is set in AccountController.Login
    // AccessPermissions is an enum in Modles/Enums.cs
    //
    // Example: perms = 29 (binary: 11101), permission = ManageFees (4, binary: 00100)
    //   11101  (29 - Jane's permissions: ManageSites+ManageReservations+ManageFees+ViewReports)
    // & 00100  (4  - ManageFees)
    // -------
    //   00100  (4  - result is non-zero, they have ManageFees permission)
    protected bool HasPermission(AccessPermissions permission)
    {
        var perms = HttpContext.Session.GetInt32("Permissions");
        if (perms == null)
            return false;
        // Check if the permission is set by typcasting perms into a AccessPermissions value, so we can use the bitewise & operator to see if they share the same bit
        return ((AccessPermissions)perms & permission) == permission;
    }

    // Redirect to login page if user is not logged in
    protected IActionResult? RequireLogin()
    {
        if (!IsLoggedIn())
            return RedirectToAction("Login", "Account");
        return null;
    }

    // Redirect to access denied page if user is not an admin
    // Can be used for edting employees, etc.
    protected IActionResult? RequireAdmin()
    {
        var redirect = RequireLogin();
        if (redirect != null)
            return redirect;
        if (!IsAdmin())
            return RedirectToAction("AccessDenied", "Account");
        return null;
    }

    // Returns null if user has permission, otherwise returns a redirect to access denied page
    // Call this anytime you want to require a user to have a certain permission to access a page
    // Look at the example in Controllers/FeesController.Index
    protected IActionResult? RequirePermission(AccessPermissions permission)
    {
        var redirect = RequireLogin();
        if (redirect != null)
            return redirect;
        if (!IsAdmin() && !HasPermission(permission))
            return RedirectToAction("AccessDenied", "Account");
        return null;
    }
}
