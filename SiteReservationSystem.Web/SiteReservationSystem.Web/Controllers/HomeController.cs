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
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            return View();
        }

        public IActionResult Privacy()
        {
            var redirect = RequireLogin();
            if (redirect != null)
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

            query = query.Where(s => s.SiteTypeID == filterSite);

            if (filterSite == 1 && length.HasValue)
            {
                query = query.Where(s => s.MaxLengthFeet > length.Value);
            }

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

            return RedirectToAction("Index", new { id = model.SiteID });
        }

        public async Task<IActionResult> MyReservations()
        {
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return Unauthorized();

            var reservations = await _context.Reservations
                .Include(r => r.Site)
                    .ThenInclude(s => s.SiteType)
                .Include(r => r.ReservationStatus)
                .Where(r => r.CustomerID == customer.CustomerID)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();

            return View(reservations);
        }

        [HttpGet]
        public async Task<IActionResult> EditReservation(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return Unauthorized();

            var reservation = await _context.Reservations
                .Include(r => r.Site)
                .Include(r => r.ReservationStatus)
                .FirstOrDefaultAsync(r => r.ReservationID == id && r.CustomerID == customer.CustomerID);

            if (reservation == null)
                return NotFound();

            ViewBag.Sites = await _context.Sites.ToListAsync();
            ViewBag.Statuses = await _context.ReservationStatuses.ToListAsync();

            return View("~/Views/Reservations/Edit.cshtml", reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReservation([Bind("ReservationID,StartDate,EndDate,SiteID,ReservationStatusID,TrailerLengthFeet,Notes")] Reservation updatedReservation)
        {
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return Unauthorized();

            ModelState.Remove("Customer");
            ModelState.Remove("Site");
            ModelState.Remove("ReservationStatus");
            ModelState.Remove("ReservationFees");
            ModelState.Remove("Invoice");
            ModelState.Remove("BaseAmount");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("BalanceDue");

            var reservation = await _context.Reservations
                .Include(r => r.ReservationFees)
                .Include(r => r.Invoice)
                    .ThenInclude(i => i!.Payments)
                .FirstOrDefaultAsync(r => r.ReservationID == updatedReservation.ReservationID && r.CustomerID == customer.CustomerID);

            if (reservation == null)
                return NotFound();

            if (updatedReservation.StartDate >= updatedReservation.EndDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
            }

            var selectedSite = await _context.Sites
                .Include(s => s.SiteType)
                .FirstOrDefaultAsync(s => s.SiteID == updatedReservation.SiteID);

            if (selectedSite == null)
            {
                ModelState.AddModelError("", "Selected site was not found.");
            }
            else
            {
                if (updatedReservation.TrailerLengthFeet.HasValue &&
                    updatedReservation.TrailerLengthFeet.Value > selectedSite.MaxLengthFeet)
                {
                    ModelState.AddModelError("", "The selected site cannot accommodate that trailer length.");
                }

                var cancelledStatusId = await _context.ReservationStatuses
                    .Where(rs => rs.StatusName == "Cancelled")
                    .Select(rs => (int?)rs.ReservationStatusID)
                    .FirstOrDefaultAsync();

                bool hasConflict = await _context.Reservations.AnyAsync(r =>
                    r.SiteID == updatedReservation.SiteID &&
                    r.ReservationID != updatedReservation.ReservationID &&
                    r.ReservationStatusID != cancelledStatusId &&
                    updatedReservation.StartDate < r.EndDate &&
                    updatedReservation.EndDate > r.StartDate);

                if (hasConflict)
                {
                    ModelState.AddModelError("", "That site is already reserved for the selected dates.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sites = await _context.Sites.ToListAsync();
                ViewBag.Statuses = await _context.ReservationStatuses.ToListAsync();
                return View("~/Views/Reservations/Edit.cshtml", updatedReservation);
            }

            reservation.StartDate = updatedReservation.StartDate;
            reservation.EndDate = updatedReservation.EndDate;
            reservation.SiteID = updatedReservation.SiteID;
            reservation.ReservationStatusID = updatedReservation.ReservationStatusID;
            reservation.TrailerLengthFeet = updatedReservation.TrailerLengthFeet;
            reservation.Notes = updatedReservation.Notes;
            reservation.LastUpdated = DateTime.UtcNow;

            int numberOfNights = (updatedReservation.EndDate - updatedReservation.StartDate).Days;

            var pricing = await _context.SiteTypePricings
                .Where(p => p.SiteTypeID == selectedSite.SiteTypeID &&
                            p.StartDate <= updatedReservation.StartDate &&
                            (p.EndDate == null || p.EndDate >= updatedReservation.StartDate))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefaultAsync();

            decimal nightlyRate = pricing?.BasePrice ?? 0m;
            decimal newTotalForNights = nightlyRate * numberOfNights;
            decimal existingFees = reservation.ReservationFees.Sum(rf => rf.Amount);
            decimal oldTotalAmount = reservation.TotalAmount;
            decimal newTotalAmount = newTotalForNights + existingFees;
            decimal priceDifference = newTotalAmount - oldTotalAmount;

            reservation.TotalAmount = newTotalAmount;

            decimal amountPaid = 0;
            if (reservation.Invoice?.Payments != null)
            {
                amountPaid = reservation.Invoice.Payments.Where(p => !p.IsRefund).Sum(p => p.Amount);
            }

            reservation.BalanceDue = reservation.TotalAmount - amountPaid;

            if (reservation.Invoice != null)
            {
                reservation.Invoice.TotalAmount = reservation.TotalAmount;
                reservation.Invoice.IsPaid = false;
                reservation.Invoice.DatePaid = null;
            }

            TempData["OldTotalAmount"] = oldTotalAmount.ToString("F2");
            TempData["NewTotalAmount"] = newTotalAmount.ToString("F2");
            TempData["PriceDifference"] = priceDifference.ToString("F2");

            await _context.SaveChangesAsync();

            return RedirectToAction("MyReservations");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return Unauthorized();

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.ReservationID == id && r.CustomerID == customer.CustomerID);

            if (reservation == null)
                return NotFound();

            var cancelledStatus = await _context.ReservationStatuses
                .FirstOrDefaultAsync(rs => rs.StatusName == "Cancelled");

            if (cancelledStatus == null)
                return NotFound();

            reservation.ReservationStatusID = cancelledStatus.ReservationStatusID;
            reservation.LastUpdated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(reservation.Notes))
                reservation.Notes = "Reservation cancelled by customer.";
            else
                reservation.Notes += " | Reservation cancelled by customer.";

            await _context.SaveChangesAsync();

            return RedirectToAction("MyReservations");
        }
    }
}