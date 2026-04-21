using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostmarkDotNet;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;
using SiteReservationSystem.Web.ViewModels;

namespace SiteReservationSystem.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }

        // Login page
        // Route is /Account/Login
        [HttpGet]
        public IActionResult Login() => View();
        public IActionResult Register() => View();

        // Checks if user is valid and logs them in, storing user info in session
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Use Bcrypt later
            if (user == null || user.PasswordHash != password)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Your account has been disabled. Contact an administrator.";
                return View();
            }

            // Store user info in session
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role.ToString());

            // Store name + role-based session data
            if (user.Role == UserRole.Admin)
            {
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.UserID == user.UserID);
                if (admin != null)
                    HttpContext.Session.SetString("Name", $"{admin.FirstName} {admin.LastName}");
            }
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
                    // IMPORTANT: link customer to session
                    HttpContext.Session.SetInt32("CustomerID", customer.CustomerID);

                    HttpContext.Session.SetString(
                        "Name",
                        $"{customer.FirstName} {customer.LastName}"
                    );

                    // REQUIRED so authorization does not block customer routes
                    HttpContext.Session.SetInt32("Permissions", 0);

                    // Military / DoD tracking
                    HttpContext.Session.SetInt32(
                        "IsPCSOrders",
                        customer.DoDStatus == DoDStatus.PCS_ORDERS ? 1 : 0
                    );

                    HttpContext.Session.SetString(
                        "MilitaryAffiliation",
                        customer.MilitaryAffiliation.ToString()
                    );

                    Console.WriteLine(
                        $"DoDStatus: {customer.DoDStatus}, MilitaryAffiliation: {customer.MilitaryAffiliation}, IsPCSOrders: {customer.DoDStatus == DoDStatus.PCS_ORDERS}"
                    );
                }
            }

            // Redirect based on role
            await UpdateReservationStatuses();

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

        // Access denied page
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST: Accounts/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel regUser)
        {

            User user = new User();
            Customer customer = new Customer();

            if (ModelState.IsValid)
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == regUser.Email);
                if (existing != null)
                {
                    ViewBag.Error = "An account with that email already exists.";
                    return View(regUser);
                }

                user.Email = regUser.Email;
                user.PasswordHash = regUser.Password;
                user.Role = UserRole.Customer;
                _context.Add(user);
                await _context.SaveChangesAsync();

                string formattedPhone = FormatNumber(regUser.Phone);

                customer.UserID = user.UserID;
                customer.FirstName = regUser.FirstName;
                customer.LastName = regUser.LastName;
                customer.PhoneNumber = formattedPhone;
                customer.MilitaryAffiliation = regUser.MilitaryAffiliation;
                customer.DoDStatus = regUser.DoDStatus;

                _context.Add(customer);
                await _context.SaveChangesAsync();

                try
                {
                    var client = new PostmarkClient(_configuration["Postmark:ServerToken"]);
                    var message = new PostmarkMessage()
                    {
                        To = user.Email,
                        From = "kelsiebridge@mail.weber.edu",
                        Subject = "RV Park Registration",
                        TextBody = $"Hello {customer.FirstName},\n\nYour account has been created. Thank you."
                    };
                    Console.WriteLine($"[EMAIL] Attempting to send to: {user.Email}");
                    var result = await client.SendMessageAsync(message);
                    Console.WriteLine($"[EMAIL] Postmark response - ErrorCode: {result.ErrorCode}, Message: {result.Message}");
                    if (result.ErrorCode != 0)
                    {
                        Console.WriteLine($"[EMAIL] FAILED - ErrorCode: {result.ErrorCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL] Exception: {ex.GetType().Name}");
                    Console.WriteLine($"[EMAIL] Message: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[EMAIL] InnerException: {ex.InnerException.Message}");
                    }
                    Console.WriteLine($"[EMAIL] StackTrace: {ex.StackTrace}");
                }
                
                TempData["Message"] = "Account created. Check your email for confirmation.";
                return RedirectToAction("Login");

            }

            return View(regUser);
        }

        public static string FormatNumber(string phone)
        {
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 10)
            {
                return string.Format("{0}-{1}-{2}",
                    digitsOnly.Substring(0, 3),
                    digitsOnly.Substring(3, 3),
                    digitsOnly.Substring(6, 4));
            }

            return digitsOnly;
        }

    }
}
