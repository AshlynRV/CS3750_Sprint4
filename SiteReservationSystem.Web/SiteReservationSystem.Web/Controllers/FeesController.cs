using Microsoft.AspNetCore.Mvc;
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


        public async Task<IActionResult> DeleteFee(int? id)
        {
            if (id == null) return NotFound();

            var fee = await _context.Fees
                .FirstOrDefaultAsync(f => f.FeeID == id);

            if (fee == null) return NotFound();

            return View(fee);
        }

        
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
    }
}
