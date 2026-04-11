using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Filters;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Controllers
{
    [Authorize(AccessPermissions.ManageSiteTypes)]
    public class SiteTypesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SiteTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Load initial page with SiteTypeID and Description for drop-down
        public IActionResult Index()
        {
            var siteTypes = _context.SiteTypes
                .Select(s => new SelectListItem
                {
                    Value = s.SiteTypeID.ToString(),
                    Text = s.Description
                }).ToList();

            ViewBag.SiteTypeList = siteTypes;
            return View(new SiteTypePricing());
        }

        //Redirect to new page, pass SiteTypeID
        [HttpPost]
        public IActionResult Index(SiteTypePricing model)
        {
            if (model.SiteTypeID == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid site type.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("UpdatePrice", new { siteTypeId = model.SiteTypeID });
        }

        //Fill out page with information from SiteTypeID
        public IActionResult UpdatePrice(int siteTypeId)
        {
            // Verify site type exists before allowing access
            var siteType = _context.SiteTypes.Find(siteTypeId);
            if (siteType == null)
            {
                TempData["ErrorMessage"] = "Site type not found.";
                return RedirectToAction("Index");
            }

            var pricing = _context.SiteTypePricings
                .FirstOrDefault(p => p.SiteTypeID == siteTypeId);

            if (pricing == null)
            {
                pricing = new SiteTypePricing { SiteTypeID = siteTypeId };
            }

            ViewBag.SiteTypeName = siteType.Description;

            return View(pricing);
        }

        //Update database with input
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePrice(SiteTypePricing model)
        {
            ModelState.Remove("SiteType");

            if (ModelState.IsValid)
            {
                // Check if this is a new pricing record or an existing one to update
                if (model.PricingID == 0) // No existing record
                {
                    if (model.StartDate == default)
                    {
                        model.StartDate = DateTime.Today;
                    }
                    _context.SiteTypePricings.Add(model);
                }
                else // Existing record found
                {
                    var existing = _context.SiteTypePricings.Find(model.PricingID);
                    if (existing != null)
                    {
                        existing.BasePrice = model.BasePrice;
                        existing.StartDate = model.StartDate == default ? existing.StartDate : model.StartDate;
                        existing.EndDate = model.EndDate;
                    }
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Site Type updated successfully.";
                TempData.Remove("ErrorMessage");
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
            TempData.Remove("SuccessMessage");
            return RedirectToAction("UpdatePrice", new { siteTypeId = model.SiteTypeID });
        }
    }
}
