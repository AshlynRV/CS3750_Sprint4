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
                var siteTypes = _context.SiteTypes
                    .Select(s => new SelectListItem
                    {
                        Value = s.SiteTypeID.ToString(),
                        Text = s.Description
                    }).ToList();
                ViewBag.SiteTypeList = siteTypes;
                return View(model);
            }

            return RedirectToAction("UpdatePrice", new { siteTypeId = model.SiteTypeID });
        }

        //Fill out page with information from SiteTypeID
        public IActionResult UpdatePrice(int siteTypeId)
        {
            var pricing = _context.SiteTypePricings
                .FirstOrDefault(p => p.SiteTypeID == siteTypeId);

            if (pricing == null)
            {
                pricing = new SiteTypePricing { SiteTypeID = siteTypeId };
            }

            ViewBag.SiteTypeName = _context.SiteTypes
                .Where(s => s.SiteTypeID == siteTypeId)
                .Select(s => s.Description)
                .FirstOrDefault();

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
                if (model.StartDate == null)
                {
                    model.StartDate = DateTime.Today;
                }
                if (model.PricingID == 0) // No existing record
                {
                    _context.SiteTypePricings.Add(model);
                }
                else // Existing record found
                {
                    var existing = _context.SiteTypePricings.Find(model.PricingID);
                    if (existing != null)
                    {
                        _context.Update(existing);
                    }

                    var newPricing = new SiteTypePricing
                    {
                        SiteTypeID = model.SiteTypeID,
                        BasePrice = model.BasePrice,
                        Description = model.Description,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };
                    _context.SiteTypePricings.Add(newPricing);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // If validation fails, repopulate ViewBag
            ViewBag.SiteTypeName = _context.SiteTypes
                .Where(s => s.SiteTypeID == model.SiteTypeID)
                .Select(s => s.Description)
                .FirstOrDefault();

            return View(model);
        }
    }
}
