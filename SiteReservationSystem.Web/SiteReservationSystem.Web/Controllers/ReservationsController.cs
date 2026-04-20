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

        public ReservationsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays all reservations and supports filtering and sorting.
        /// </summary>
        public async Task<IActionResult> Index(int? reservationId, string? customerName, DateTime? startDate, DateTime? endDate, string? sortOrder, int? filterStatus)
        {
            // Clear TempData messages so they don't show again on page refresh
            TempData.Remove("FeeMessage");

            await UpdateReservationStatuses();
            TempData.Remove("OldTotalAmount");
            TempData.Remove("PriceDifference");

            // Build query with includes for related data
            var query = _context.Reservations
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .Include(r => r.Site)
                .Include(r => r.ReservationStatus)
                .Include(r => r.Invoice)
                .AsQueryable();

            // Apply filters based on query parameters
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

            // Filter by reservation status (dropdown filter: Upcoming, In Progress, Completed, Cancelled)
            if (filterStatus.HasValue)
                query = query.Where(r => r.ReservationStatusID == filterStatus.Value);

            if (sortOrder == "status")
                query = query.OrderBy(r => r.ReservationStatus.StatusName);

            var reservations = query.ToList();
            return View(reservations);
        }

        // Details Action
        public IActionResult Details(int id)
        {
            // Fetch reservation with all related data needed for display
            var reservation = _context.Reservations
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)
                .Include(r => r.Site)
                    .ThenInclude(s => s.SiteType)
                        .ThenInclude(st => st.SiteTypePricings) // For Current Stay display
                .Include(r => r.ReservationStatus)
                .Include(r => r.ReservationFees)
                    .ThenInclude(rf => rf.Fee) // For fee breakdown div
                .Include(r => r.Invoice)
                    .ThenInclude(i => i!.Payments) // For balance calculation
                .FirstOrDefault(r => r.ReservationID == id);

            if (reservation == null)
                return NotFound();

            // Get current price per night based on reservation start date
            // This is used in the view to show "num nights x $price"
            var pricing = reservation.Site.SiteType.SiteTypePricings
                .Where(p => p.StartDate <= reservation.StartDate && (p.EndDate == null || p.EndDate >= reservation.StartDate))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            ViewBag.CurrentPricePerNight = pricing?.BasePrice ?? 0;

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

        // POST: Edit Reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("ReservationID,StartDate,EndDate,SiteID,ReservationStatusID,TrailerLengthFeet,Notes")] Reservation updatedReservation)
        {
            // Skip validation for navigation/computed properties
            ModelState.Remove("Customer");
            ModelState.Remove("Site");
            ModelState.Remove("ReservationStatus");
            ModelState.Remove("ReservationFees");
            ModelState.Remove("Invoice");

            // We calculate these fields
            ModelState.Remove("BaseAmount");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("BalanceDue");

            // Fetch existing reservation with fees and invoice for recalculation
            var reservation = _context.Reservations
                .Include(r => r.ReservationFees)
                .Include(r => r.Invoice)
                    .ThenInclude(i => i!.Payments)
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

            // Update basic reservation fields
            reservation.StartDate = updatedReservation.StartDate;
            reservation.EndDate = updatedReservation.EndDate;
            reservation.SiteID = updatedReservation.SiteID;
            reservation.ReservationStatusID = updatedReservation.ReservationStatusID;
            reservation.TrailerLengthFeet = updatedReservation.TrailerLengthFeet;
            reservation.LastUpdated = DateTime.UtcNow;

            // Recalculate Total Amount: TotalAmount = (current nights × current price) + fees
            // BaseAmount stays the same (original booking price never changes)
            int numberOfNights = (updatedReservation.EndDate - updatedReservation.StartDate).Days;

            // Get current price for this site type based on reservation start date
            var pricing = _context.SiteTypePricings
                .Where(p => p.SiteTypeID == selectedSite.SiteTypeID &&
                            p.StartDate <= updatedReservation.StartDate &&
                            (p.EndDate == null || p.EndDate >= updatedReservation.StartDate))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            decimal nightlyRate = pricing?.BasePrice ?? 0m;
            decimal newTotalForNights = nightlyRate * numberOfNights;
            decimal existingFees = reservation.ReservationFees.Sum(rf => rf.Amount);
            decimal oldTotalAmount = reservation.TotalAmount;
            decimal newTotalAmount = newTotalForNights + existingFees;
            decimal priceDifference = newTotalAmount - oldTotalAmount;

            // Update reservation with new total
            reservation.TotalAmount = newTotalAmount;

            // Recalculate BalanceDue: AmountPaid = sum of all non-refund payments
            decimal amountPaid = 0;
            if (reservation.Invoice?.Payments != null)
            {
                amountPaid = reservation.Invoice.Payments.Where(p => !p.IsRefund).Sum(p => p.Amount);
            }
            reservation.BalanceDue = reservation.TotalAmount - amountPaid;

            // Sync Invoice with Reservation
            if (reservation.Invoice != null)
            {
                reservation.Invoice.TotalAmount = reservation.TotalAmount;
                reservation.Invoice.IsPaid = false;
                reservation.Invoice.DatePaid = null;
            }

            // Store values for display message on Details page
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
                reservation.Notes = "Reservation cancelled by admin/employee.";
            else
                reservation.Notes += " | Reservation cancelled by admin/employee.";

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }
    }
}
