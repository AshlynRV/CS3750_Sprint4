using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;
using SiteReservationSystem.Web.ViewModels;
using System.Diagnostics;

namespace SiteReservationSystem.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Redirect to login page if user is not logged in. This is a function in BaseController.cs
            var redirect = RequireLogin();
            if (redirect != null)
                // Redirects to login page
                return redirect;

            return View();
        }

        public IActionResult Privacy()
        {
            var redirect = RequireLogin();
            if (redirect != null)
                // Redirects to login page
                return redirect;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> CreateReservation()
        {
            return View();
        }

        public async Task<IActionResult> Search(DateTime startDate, DateTime endDate, int? filterSite, int? length)
        {
            var query = _context.Sites.Include(s => s.SitePhotos).AsQueryable();

            // Filter by type
            query = query.Where(s => s.SiteTypeID == filterSite);

            // Filter by length only if it's provided (Bring own Unit)
            if (filterSite == 1 && length.HasValue)
            {
                query = query.Where(s => s.MaxLengthFeet > length.Value);
            }

            // 2. Filter out sites that have any overlapping reservations
            // Overlap logic: (StartA < EndB) AND (EndA > StartB)
            var availableSites = await query
                .Where(site => !site.Reservations.Any(r =>
                    startDate < r.EndDate && endDate > r.StartDate))
                .ToListAsync();
            
            return View("CreateReservation", availableSites);
        }

        [HttpGet]
        public async Task<IActionResult> Reserve(int siteId, DateTime startDate, DateTime endDate, int? length)
        {
            var site = await _context.Sites.FindAsync(siteId);

            var model = new CreateReservationViewModel
            {
                SiteID = siteId,
                StartDate = startDate,
                EndDate = endDate,
                TrailerLengthFeet = length ?? 0
                
            };
           
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Get UserID from Session (more reliable with your current setup)
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return Unauthorized();

            var reservation = new Reservation
            {
                SiteID = model.SiteID,
                CustomerID = customer.CustomerID,
                ReservationStatusID = 1,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TrailerLengthFeet = model.TrailerLengthFeet,
                NumberOfGuests = model.NumberOfGuests,
                NumberOfPets = model.NumberOfPets,
                Notes = model.SpecialRequests,
                //needs to be update to have the correct pricing according to site and dates
                BaseAmount = 0,
                TotalAmount = 0,
                BalanceDue = 0,
                ScheduledCheckInTime = model.StartDate,
                ScheduledCheckOutTime = model.EndDate,
                DateCreated = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("index", new { id = model.SiteID });
        }


    }
}
