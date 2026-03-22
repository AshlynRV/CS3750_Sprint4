using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Controllers
{
    public class FeesController : BaseController
    {
        // Declare DbContext
        private readonly ApplicationDbContext _context;

        // Inject DbContext into controller
        public FeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Only admins and employees with ManageFees permission can access this page
        public async Task<IActionResult> Index()
        {
            // Redirect to access denied page if user doesn't have ManageFees permission
            // This is a function in BaseController.cs
            var redirect = RequirePermission(AccessPermissions.ManageFees);
            if (redirect != null)
                // Redirects to access denied page
                return redirect;

            ViewBag.Fees = await _context.Fees.ToListAsync();
            return View();
        }
    }
}
