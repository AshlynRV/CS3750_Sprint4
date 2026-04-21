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
        public SitesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // -------------------------
        // CUSTOMER ACCESSIBLE
        // -------------------------
        public async Task<IActionResult> Index()
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Customer" && userRole != "Admin" && !HasPermission(AccessPermissions.ManageSites))
                return RedirectToAction("AccessDenied", "Account");

            var sites = await _context.Sites
                .Include(s => s.SiteType)
                .Include(s => s.SitePhotos)
                .ToListAsync();
            return View(sites);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var redirect = RequireLogin();
            if (redirect != null) return redirect;
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Customer" && userRole != "Admin" && !HasPermission(AccessPermissions.ManageSites))
                return RedirectToAction("AccessDenied", "Account");

            if (id == null)
                return NotFound();

            await UpdateReservationStatuses();

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
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Customer")
            {
                return StatusCode(401, "Only customers can make reservations.");
            }

            var site = await _context.Sites.FindAsync(id);
            if (site == null) return NotFound();

            ViewBag.MaxLengthFeet = site.MaxLengthFeet;

            var model = new CreateReservationViewModel
            {
                SiteID = id,
                SiteNumber = site.SiteNumber,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var today = DateTime.Today;
            if (model.StartDate < today)
            {
                ModelState.AddModelError("", "Start date cannot be in the past.");
                return View(model);
            }

            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
                return View(model);
            }

            var isPCS = HttpContext.Session.GetInt32("IsPCSOrders") == 1;
            var stayLength = (model.EndDate - model.StartDate).Days;
            var isPeakSeason = model.StartDate >= new DateTime(model.StartDate.Year, 4, 1) && model.StartDate <= new DateTime(model.StartDate.Year, 10, 15);

            if (!isPCS && isPeakSeason && stayLength > 14)
            {
                ModelState.AddModelError("", "Maximum stay is 14 days during peak season.");
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            var customerEmail = customer.User?.Email ?? "";
            var customerPhone = customer.PhoneNumber ?? "";

            var site = await _context.Sites
                .Include(s => s.SiteType)
                    .ThenInclude(st => st.SiteTypePricings)
                .FirstOrDefaultAsync(s => s.SiteID == model.SiteID);

            if (site == null)
                return NotFound();

            int numberOfNights = (model.EndDate - model.StartDate).Days;

            var pricing = site.SiteType.SiteTypePricings
                .Where(p => p.StartDate <= model.StartDate && (p.EndDate == null || p.EndDate >= model.StartDate))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            decimal nightlyRate = pricing?.BasePrice ?? 0m;
            decimal totalAmount = nightlyRate * numberOfNights;

            HttpContext.Session.SetString("ReservationData", System.Text.Json.JsonSerializer.Serialize(new
            {
                SiteID = model.SiteID,
                SiteNumber = site.SiteNumber,
                CustomerID = customer.CustomerID,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TrailerLengthFeet = model.TrailerLengthFeet,
                NumberOfGuests = model.NumberOfGuests,
                NumberOfPets = model.NumberOfPets,
                SpecialRequests = model.SpecialRequests,
                TotalAmount = totalAmount,
                NumberOfNights = numberOfNights,
                PricePerNight = nightlyRate,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone
            }));

            return RedirectToAction("CreateReservationPayment", "Payments");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> EditPhoto(int id, SitePhoto photo)
        {
            if (id != photo.PhotoID) return NotFound();
            var existing = await _context.SitePhotos
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.PhotoID == id);
            if (existing == null) return NotFound();
            existing.PhotoURL = photo.PhotoURL;
            existing.Caption = photo.Caption;
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = existing.SiteID });
        }

        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> DeletePhoto(int? id)
        {
            if (id == null) return NotFound();
            var photo = await _context.SitePhotos
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.PhotoID == id);
            if (photo == null) return NotFound();
            return View(photo);
        }

        [HttpPost, ActionName("DeletePhoto")]
        [ValidateAntiForgeryToken]
        [Authorize(AccessPermissions.ManageSites)]
        public async Task<IActionResult> DeletePhotoConfirmed(int id)
        {
            var photo = await _context.SitePhotos.FindAsync(id);
            var siteID = 0;
            if (photo != null) siteID = photo.SiteID;
            if (photo != null)
            {
                _context.SitePhotos.Remove(photo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = siteID });
        }
    }
}
