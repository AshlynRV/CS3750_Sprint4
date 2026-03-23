using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Controllers
{
    public class SitesController : BaseController
    {
        // Declare DbContext
        private readonly ApplicationDbContext _context;

        // Inject DbContext into controller
        public SitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Only admins and employees with ManageSites permission can access this page
        public async Task<IActionResult> Index()
        {
            // Redirect to access denied page if user doesn't have ManageSites permission
            // This is a function in BaseController.cs
            var redirect = RequirePermission(AccessPermissions.ManageSites);
            if (redirect != null)
                // Redirects to access denied page
                return redirect;

            // Get site type and photos from proper models
            var sites = await _context.Sites
                .Include(s => s.SiteType)
                .Include(s => s.SitePhotos)
                .ToListAsync();
            return View(sites);
        }

        // Details page for each site
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get info including type, photos and reservations for site
            var site = await _context.Sites
                .Include(s => s.SiteType)
                .Include(s => s.SitePhotos)
                .Include(s => s.Reservations)
                    .ThenInclude(r => r.ReservationStatus)
                .Include(s => s.Reservations)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync(s => s.SiteID == id);

            if (site == null)
            {
                return NotFound();
            }

            return View(site);
        }


        // CRUD for Sites

        // GET: Sites/CreateSite
        public async Task<IActionResult> CreateSite()
        {
            ViewBag.SiteTypes = await _context.SiteTypes.ToListAsync();
            return View();
        }

        // POST: Sites/CreateSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSite(Site site)
        {
            // Site Type handled differently, SiteID automatic
            ModelState.Remove("SiteType");
            ModelState.Remove("SiteID");

            if (ModelState.IsValid)
            {
                _context.Add(site);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Get proper list of Site Types 
            ViewBag.SiteTypes = await _context.SiteTypes.ToListAsync();
            return View(site);
        }

        // GET: Sites/EditSite/5
        public async Task<IActionResult> EditSite(int? id)
        {
            if (id == null) return NotFound();

            var site = await _context.Sites.FindAsync(id);
            if (site == null) return NotFound();

            ViewBag.SiteTypes = await _context.SiteTypes.ToListAsync();

            return View(site);
        }

        // POST: Sites/EditSite/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSite(int id, Site site)
        {
            ModelState.Remove("SiteType");

            if (id != site.SiteID) return NotFound();

            var existing = await _context.Sites.FindAsync(id);
            if (existing == null) return NotFound();

            existing.SiteNumber = site.SiteNumber;
            existing.SiteTypeID = site.SiteTypeID;
            existing.MaxLengthFeet = site.MaxLengthFeet;

            if (!string.IsNullOrEmpty(site.Notes))
            {
                existing.Notes = site.Notes;
            } else
            {
                existing.Notes = null;
            }

            existing.IsActive = site.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Sites/DeleteSite/5
        public async Task<IActionResult> DeleteSite(int? id)
        {
            if (id == null) return NotFound();

            var site = await _context.Sites
                .FirstOrDefaultAsync(m => m.SiteID == id);

            if (site == null) return NotFound();

            return View(site);
        }

        // POST: Sites/DeleteSite/5
        [HttpPost, ActionName("DeleteSite")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSiteConfirmed(int id)
        {

            // Find the site
            var site = await _context.Sites.FindAsync(id);

            // Only remove if not null (fixes warning)
            if (site != null)
            {
                _context.Sites.Remove(site);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // CRUD for Site Photos

        // GET: Sites/CreatePhoto?siteID=1
        public async Task<IActionResult> CreatePhoto(int? siteID)
        {
            if (siteID == null) return NotFound();

            var site = await _context.Sites.FindAsync(siteID);
            if (site == null) return NotFound();

            return View(new SitePhoto { SiteID = siteID.Value });
        }

        // POST: Sites/CreatePhoto?siteID=1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePhoto(SitePhoto photo)
        {

            ModelState.Remove("Site");

            if (ModelState.IsValid)
            {
                photo.DateUploaded = DateTime.UtcNow;

                // Get and set correct new DisplayOrder value
                var existingPhotos = await _context.SitePhotos
                    .Where(p => p.SiteID == photo.SiteID)
                    .ToListAsync();
                var maxDisplayOrder = 0;
                if (existingPhotos.Any())
                {
                    maxDisplayOrder = existingPhotos.Max(p => p.DisplayOrder);
                }
                photo.DisplayOrder = maxDisplayOrder + 1;

                _context.Add(photo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = photo.SiteID });
            }
            return View(photo);
        }

        // GET: Sites/EditPhoto/5
        public async Task<IActionResult> EditPhoto(int? id)
        {
            if (id == null) return NotFound();

            var photo = await _context.SitePhotos
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.PhotoID == id);
            if (photo == null) return NotFound();

            return View(photo);
        }

        // POST: Sites/EditPhoto/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(int id, SitePhoto photo)
        {
            if (id != photo.PhotoID) return NotFound();

            var existing = await _context.SitePhotos
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.PhotoID == id);
            if (existing == null) return NotFound();

            // Only need to change URL and/or Caption
            existing.PhotoURL = photo.PhotoURL;
            existing.Caption = photo.Caption;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = existing.SiteID });
        }

        // GET: Sites/DeletePhoto/5
        public async Task<IActionResult> DeletePhoto(int? id)
        {
            if (id == null) return NotFound();

            var photo = await _context.SitePhotos
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.PhotoID == id);

            if (photo == null) return NotFound();

            return View(photo);
        }

        // POST: Sites/DeletePhoto/5
        [HttpPost, ActionName("DeletePhoto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhotoConfirmed(int id)
        {

            // Find the photo
            var photo = await _context.SitePhotos.FindAsync(id);

            var siteID = 0;

            if (photo != null)
            {
                siteID = photo.SiteID;
            }

            // Only remove if not null (fixes warning)
            if (photo != null)
            {
                _context.SitePhotos.Remove(photo);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = siteID });
        }
    }

}
