using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Filters;
using SiteReservationSystem.Web.Models;
using SiteReservationSystem.Web.ViewModels;

namespace SiteReservationSystem.Web.Controllers
{
    [Authorize] // keeps login required for everything
    public class SitesController : BaseController
    {
        // Declare DbContext
        private readonly ApplicationDbContext _context;

        // Inject DbContext into controller
        public SitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------------
        // CUSTOMER ACCESSIBLE
        // -------------------------
        public async Task<IActionResult> Index()
        {
            var sites = await _context.Sites
                .Include(s => s.SiteType)
                .Include(s => s.SitePhotos)
                .ToListAsync();
            return View(sites);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var site = await _context.Sites
                .Include(s => s.SiteType)
                .Include(s => s.SitePhotos)
                .Include(s => s.Reservations)
                    .ThenInclude(r => r.ReservationStatus)
                .Include(s => s.Reservations)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(s => s.SiteID == id);

            if (site == null)
                return NotFound();

            return View(site);
        }

        [HttpGet]
        public async Task<IActionResult> Reserve(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site == null) return NotFound();

            var model = new CreateReservationViewModel
            {
                SiteID = id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
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

            return RedirectToAction("Details", new { id = model.SiteID });
        }

        // -------------------------
        // ADMIN / EMPLOYEE ONLY
        // -------------------------
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> CreateSite()
        {
            ViewBag.SiteTypes = await _context.SiteTypes.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> CreateSite(Site site)
        {
            ModelState.Remove("SiteType");
            ModelState.Remove("SiteID");
            if (ModelState.IsValid)
            {
                _context.Add(site);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SiteTypes = await _context.SiteTypes.ToListAsync();
            return View(site);
        }

        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> EditSite(int? id)
        {
            if (id == null) return NotFound();
            var site = await _context.Sites.FindAsync(id);
            if (site == null) return NotFound();
            ViewBag.SiteTypes = await _context.SiteTypes.ToListAsync();
            return View(site);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> EditSite(int id, Site site)
        {
            ModelState.Remove("SiteType");
            if (id != site.SiteID) return NotFound();
            var existing = await _context.Sites.FindAsync(id);
            if (existing == null) return NotFound();
            existing.SiteNumber = site.SiteNumber;
            existing.SiteTypeID = site.SiteTypeID;
            existing.MaxLengthFeet = site.MaxLengthFeet;
            existing.Notes = string.IsNullOrEmpty(site.Notes) ? null : site.Notes;
            existing.IsActive = site.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> DeleteSite(int? id)
        {
            if (id == null) return NotFound();
            var site = await _context.Sites.FirstOrDefaultAsync(m => m.SiteID == id);
            if (site == null) return NotFound();
            return View(site);
        }

        [HttpPost, ActionName("DeleteSite")]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> DeleteSiteConfirmed(int id)
        {
            var site = await _context.Sites.FindAsync(id);
            if (site != null)
            {
                _context.Sites.Remove(site);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // -------------------------
        // SITE PHOTOS (ADMIN ONLY)
        // -------------------------
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> CreatePhoto(int? siteID)
        {
            if (siteID == null) return NotFound();
            var site = await _context.Sites.FindAsync(siteID);
            if (site == null) return NotFound();
            return View(new SitePhoto { SiteID = siteID.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> CreatePhoto(SitePhoto photo)
        {
            ModelState.Remove("Site");
            if (ModelState.IsValid)
            {
                photo.DateUploaded = DateTime.UtcNow;
                var maxDisplayOrder = await _context.SitePhotos
                    .Where(p => p.SiteID == photo.SiteID)
                    .MaxAsync(p => (int?)p.DisplayOrder) ?? 0;
                photo.DisplayOrder = maxDisplayOrder + 1;
                _context.Add(photo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = photo.SiteID });
            }
            return View(photo);
        }

        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> EditPhoto(int? id)
        {
            if (id == null) return NotFound();
            var photo = await _context.SitePhotos
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.PhotoID == id);
            if (photo == null) return NotFound();
            return View(photo);
        }
    }
}