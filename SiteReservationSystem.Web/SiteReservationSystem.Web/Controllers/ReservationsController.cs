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

        // ✅ POST: Edit Reservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Reservation updatedReservation)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.ReservationID == updatedReservation.ReservationID);

            if (reservation == null)
                return NotFound();

            reservation.StartDate = updatedReservation.StartDate;
            reservation.EndDate = updatedReservation.EndDate;
            reservation.SiteID = updatedReservation.SiteID;
            reservation.ReservationStatusID = updatedReservation.ReservationStatusID;
            reservation.LastUpdated = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction("Index");
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