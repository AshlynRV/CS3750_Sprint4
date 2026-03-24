using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;

// Connect to database, load reservations, load customer info, send to view

namespace SiteReservationSystem.Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays all reservations, also supports filtering by reservation number,
        /// customer name, and date range. If no filters are applied, all reservations are shown.
        /// </summary>
        /// <param name="reservationId"></param>
        /// <param name="customerName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IActionResult Index(int? reservationId, string? customerName, DateTime? startDate, DateTime? endDate)
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

            var reservations = query.ToList();

            return View(reservations);
        }
    }
}
