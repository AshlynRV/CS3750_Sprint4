using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Filters;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Controllers
{
    [Authorize(AccessPermissions.ManageReservations)]
    public class ReservationsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays all reservations and supports filtering and sorting.
        /// </summary>
        public IActionResult Index(int? reservationId, string? customerName, DateTime? startDate, DateTime? endDate, string? sortOrder)
        {
            var query = _context.Reservations
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .Include(r => r.Site)
                .Include(r => r.ReservationStatus)
                .AsQueryable();

            if (reservationId.HasValue)
                query = query.Where(r => r.ReservationID == reservationId.Value);

            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(r =>
                    r.Customer.FirstName.Contains(customerName) ||
                    r.Customer.LastName.Contains(customerName));

            if (startDate.HasValue)
                query = query.Where(r => r.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.EndDate <= endDate.Value);

            if (sortOrder == "status")
                query = query.OrderBy(r => r.ReservationStatus.StatusName);

            var reservations = query.ToList();

            return View(reservations);
        }

        // Details Action
        public IActionResult Details(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .Include(r => r.Site)
                    .ThenInclude(s => s.SiteType)
                .Include(r => r.ReservationStatus)
                .Include(r => r.ReservationFees)
                    .ThenInclude(rf => rf.Fee)
                .FirstOrDefault(r => r.ReservationID == id);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // GET: Edit Reservation
        public IActionResult Edit(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Site)
                .Include(r => r.ReservationStatus)
                .FirstOrDefault(r => r.ReservationID == id);

            if (reservation == null)
                return NotFound();

            ViewBag.Sites = _context.Sites.ToList();
            ViewBag.Statuses = _context.ReservationStatuses.ToList();

            return View(reservation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Reservation updatedReservation)
        {
            var reservation = _context.Reservations
                .Include(r => r.ReservationFees)
                .FirstOrDefault(r => r.ReservationID == updatedReservation.ReservationID);

            if (reservation == null)
                return NotFound();

            if (updatedReservation.StartDate >= updatedReservation.EndDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
            }

            var selectedSite = _context.Sites
                .Include(s => s.SiteType)
                .FirstOrDefault(s => s.SiteID == updatedReservation.SiteID);

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

                var cancelledStatusId = _context.ReservationStatuses
                    .FirstOrDefault(rs => rs.StatusName == "Cancelled")?.ReservationStatusID;

                bool hasConflict = _context.Reservations.Any(r =>
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
                ViewBag.Sites = _context.Sites.ToList();
                ViewBag.Statuses = _context.ReservationStatuses.ToList();
                return View(updatedReservation);
            }

            reservation.StartDate = updatedReservation.StartDate;
            reservation.EndDate = updatedReservation.EndDate;
            reservation.SiteID = updatedReservation.SiteID;
            reservation.ReservationStatusID = updatedReservation.ReservationStatusID;
            reservation.TrailerLengthFeet = updatedReservation.TrailerLengthFeet;
            reservation.LastUpdated = DateTime.UtcNow;

            // Recalculate pricing
            int numberOfNights = (updatedReservation.EndDate - updatedReservation.StartDate).Days;

            var pricing = _context.SiteTypePricings
                .Where(p => p.SiteTypeID == selectedSite.SiteTypeID &&
                            p.StartDate <= updatedReservation.StartDate &&
                            (p.EndDate == null || p.EndDate >= updatedReservation.StartDate))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            decimal nightlyRate = pricing?.BasePrice ?? 0m;
            decimal newBaseAmount = nightlyRate * numberOfNights;
            decimal existingFees = reservation.ReservationFees.Sum(rf => rf.Amount);
            decimal oldTotalAmount = reservation.TotalAmount;
            decimal newTotalAmount = newBaseAmount + existingFees;
            decimal priceDifference = newTotalAmount - oldTotalAmount;

            reservation.BaseAmount = newBaseAmount;
            reservation.TotalAmount = newTotalAmount;

            TempData["OldTotalAmount"] = oldTotalAmount.ToString("F2");
            TempData["NewTotalAmount"] = newTotalAmount.ToString("F2");
            TempData["PriceDifference"] = priceDifference.ToString("F2");

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = reservation.ReservationID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.ReservationID == id);

            if (reservation == null)
                return NotFound();

            var cancelledStatus = _context.ReservationStatuses
                .FirstOrDefault(rs => rs.StatusName == "Cancelled");

            if (cancelledStatus == null)
                return NotFound();

            reservation.ReservationStatusID = cancelledStatus.ReservationStatusID;
            reservation.LastUpdated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(reservation.Notes))
                reservation.Notes = "Reservation cancelled by guest.";
            else
                reservation.Notes += " | Reservation cancelled by guest.";

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> Details(int? id) 
        { 
            if (id == null) return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.ReservationFees)
                .ThenInclude(rf => rf.Fee)
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null ) return NotFound();

            return View(reservation);
        }
    }
}