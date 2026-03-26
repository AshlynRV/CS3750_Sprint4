using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Controllers
{
    public class FeesController : BaseController
    {
        // Declare DbContext
        private readonly ApplicationDbContext _context;

        // Inject DbContext into controller
        public FeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Only admins and employees with ManageFees permission can access this page
        public async Task<IActionResult> Index()
        {
            // Redirect to access denied page if user doesn't have ManageFees permission
            // This is a function in BaseController.cs
            var redirect = RequirePermission(AccessPermissions.ManageFees);
            if (redirect != null)
                // Redirects to access denied page
                return redirect;

            ViewBag.Fees = await _context.Fees.ToListAsync();
            return View();
        }

        /// <summary>
        /// Returns the view for creating a new fee.
        /// </summary>
        /// <returns>A view that allows the user to enter details for a new fee.</returns>
        public IActionResult CreateFee()
        {
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request to create a new fee record.
        /// </summary>
        /// <remarks>This action requires a valid anti-forgery token and model state validation. If model
        /// validation fails, the user is presented with the form to correct input errors.</remarks>
        /// <param name="fee">The fee entity to add to the database. Must contain valid data as defined by the model.</param>
        /// <returns>A redirect to the Index view if the creation is successful; otherwise, the Create view with validation
        /// errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFee(Fee fee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fee);
        }

        /// <summary>
        /// Displays the edit form for the specified fee.
        /// </summary>
        /// <param name="id">The identifier of the fee to edit. Must not be null.</param>
        /// <returns>An <see cref="IActionResult"/> that renders the edit view for the fee if found; otherwise, a NotFound
        /// result.</returns>
        public async Task<IActionResult> EditFee(int? id)
        {
            if (id == null) return NotFound();

            var fee = await _context.Fees.FindAsync(id);
            if (fee == null) return NotFound();

            return View(fee);
        }

        /// <summary>
        /// Handles the HTTP POST request to update an existing fee record with the specified values.
        /// </summary>
        /// <remarks>If the fee does not exist or the provided id does not match the fee's FeeID, a
        /// NotFound result is returned. Handles concurrency exceptions that may occur if the fee is modified or deleted
        /// by another user.</remarks>
        /// <param name="id">The unique identifier of the fee to update. Must match the FeeID of the provided fee object.</param>
        /// <param name="fee">The updated fee data to apply. Must have a FeeID matching the specified id parameter.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an IActionResult that redirects
        /// to the index view on success, returns a NotFound result if the fee does not exist, or redisplays the edit
        /// view if the model state is invalid.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFee(int id, Fee fee)
        {
            if (id != fee.FeeID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Fees.Any(f => f.FeeID == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fee);
        }

        /// <summary>
        /// Toggles the active status of the fee with the specified identifier and saves the change.
        /// </summary>
        /// <param name="id">The unique identifier of the fee to update.</param>
        /// <returns>A redirect to the Index view if the operation succeeds; otherwise, a NotFound result if the fee does not
        /// exist.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var fee = await _context.Fees
                .FirstOrDefaultAsync(f => f.FeeID == id);

            if (fee == null)
                return NotFound();

            fee.IsActive = !fee.IsActive;
            

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the confirmation view for deleting a fee with the specified identifier.
        /// </summary>
        /// <remarks>This method does not perform the actual deletion. It presents a confirmation view to
        /// the user before deletion occurs.</remarks>
        /// <param name="id">The identifier of the fee to delete. If null, the method returns a 404 Not Found result.</param>
        /// <returns>A view displaying the fee to be deleted if found; otherwise, a 404 Not Found result.</returns>
        public async Task<IActionResult> DeleteFee(int? id)
        {
            if (id == null) return NotFound();

            var fee = await _context.Fees
                .FirstOrDefaultAsync(f => f.FeeID == id);

            if (fee == null) return NotFound();

            return View(fee);
        }

        /// <summary>
        /// Deletes the fee with the specified identifier and redirects to the index view.
        /// </summary>
        /// <remarks>If the specified fee does not exist, no action is taken and the user is redirected to
        /// the index view. This action requires a valid anti-forgery token.</remarks>
        /// <param name="id">The unique identifier of the fee to delete.</param>
        /// <returns>A redirect to the index action after the fee is deleted.</returns>
        [HttpPost, ActionName("DeleteFee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            
            var fee = await _context.Fees.FindAsync(id);

            
            if (fee != null)
            {
                _context.Fees.Remove(fee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddFee(int reservationId)
        {
            var reservation = _context.Reservations
                .Include(r => r.Customer)
                .FirstOrDefault(r => r.ReservationID == reservationId);
            if(reservation == null)
            {
                return NotFound();
            }
            ViewBag.ReservationId = reservation.ReservationID;

            var model = new ReservationFee { ReservationID = reservationId , Reservation =reservation };

            var fees = _context.Fees.ToList();
            ViewBag.AllFees = fees;
            ViewBag.FeeList = new SelectList(_context.Fees,"FeeID", "FeeName");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeeAsync(ReservationFee reservationfee) 
        {
            ModelState.Remove("Reservation");
            ModelState.Remove("Fee");

            if (ModelState.IsValid)
            { 
                var reservation = await _context.Reservations.FindAsync(reservationfee.ReservationID);
                if( reservation !=null) 
                {
                    reservation.BalanceDue += reservationfee.Amount;

                    _context.ReservationFees.Add(reservationfee);
                    await _context.SaveChangesAsync();

                    TempData["FeeMessage"] = $"Fee successfully applied to the reservation! New balance: ${reservation.BalanceDue:N2}";

                    return RedirectToAction("Index", "Reservations", new { id = reservationfee.ReservationID });
                }
            }
            reservationfee.Reservation = await _context.Reservations
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ReservationID == reservationfee.ReservationID);

            ViewBag.FeeList = new SelectList(_context.Fees, "FeeID", "FeeName", reservationfee.FeeID);

            return View(reservationfee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReservationFee(int id)
        {
            var reservationFee = await _context.ReservationFees
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(r => r.ReservationFeeID == id);

            if (reservationFee == null) return NotFound();

            reservationFee.Reservation.BalanceDue -= reservationFee.Amount;

            _context.ReservationFees.Remove(reservationFee);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Details", "Reservations", new { id = reservationFee.ReservationID });
        }
    }
}
