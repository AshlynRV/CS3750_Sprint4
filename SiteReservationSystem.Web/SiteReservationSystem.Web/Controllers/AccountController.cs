using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Login page
        // Route is /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // Checks if user is valid and logs them in, storing user info in session
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Use Bcrypt later
            // If password doesn't match, return error to login page
            if (user == null || user.PasswordHash != password)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            // If user is not active, return error to login page and don't log them in
            if (!user.IsActive)
            {
                ViewBag.Error = "Your account has been disabled. Contact an administrator.";
                return View();
            }

            // Store user info in session
            // Can be accesses through HttpContext.Session.GetInt32("UserID")
            // Can be accesses through HttpContext.Session.GetString("UserRole")
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
            }

            // Store name based on role
            // Stores permissions for employees
            // Can be accesses through HttpContext.Session.GetString("Name")
            if (user.Role == UserRole.Admin)
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserID == user.UserID);
                if (admin != null)
                    HttpContext.Session.SetString("Name", $"{admin.FirstName} {admin.LastName}");
            }
            // Can be accesses through HttpContext.Session.GetInt32("Permissions")
            else if (user.Role == UserRole.Employee)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e =>
                    e.UserID == user.UserID
                );
                if (employee == null || employee.IsLockedOut)
                {
                    ViewBag.Error = "Your account has been locked. Contact an administrator.";
                    return View();
                }
                HttpContext.Session.SetString("Name", $"{employee.FirstName} {employee.LastName}");
                HttpContext.Session.SetInt32("Permissions", (int)employee.AccessPermissions);
            }
            else if (user.Role == UserRole.Customer)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c =>
                    c.UserID == user.UserID
                );
                if (customer != null)
                {
                    HttpContext.Session.SetString(
                        "Name",
                        $"{customer.FirstName} {customer.LastName}"
                    );
                    // Store DoD status and military affiliation
                    // Access later:
                    // var isPCSOrders = HttpContext.Session.GetInt32("IsPCSOrders") == 1;
                    // this sets it to true or false
                    HttpContext.Session.SetInt32(
                        "IsPCSOrders",
                        customer.DoDStatus == DoDStatus.PCS_ORDERS ? 1 : 0
                    );
                    HttpContext.Session.SetString(
                        "MilitaryAffiliation",
                        customer.MilitaryAffiliation.ToString()
                    );
                    Console.WriteLine($"DoDStatus: {customer.DoDStatus}, MilitaryAffiliation: {customer.MilitaryAffiliation}, IsPCSOrders: {customer.DoDStatus == DoDStatus.PCS_ORDERS}");
                }
            }

            // Redirect based on role can be done here if we want to change this later
            return user.Role switch
            {
                UserRole.Admin => RedirectToAction("Index", "Admin"),
                UserRole.Employee => RedirectToAction("Index", "Home"),
                UserRole.Customer => RedirectToAction("Index", "Home"),
            };
        }

        // Logs a user out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // This is for when a user tries to access a page they don't have permission for
        // Route is /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
