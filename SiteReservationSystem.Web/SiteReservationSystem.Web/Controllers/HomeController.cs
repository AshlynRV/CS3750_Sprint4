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

        public HomeController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Redirect to login page if user is not logged in. This is a function in BaseController.cs
            var redirect = RequireLogin();
            if (redirect != null)
                // Redirects to login page
                return redirect;

            await UpdateReservationStatuses();

            // Now giving data for homepage
            var sites = await _context.Sites
            .Include(e => e.SiteType)
            .Include(e => e.SitePhotos)
            .Include(e => e.Reservations)
            .ThenInclude(r => r.ReservationStatus)
            .ToListAsync();

            return View(sites);
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
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            return View();
        }

        public async Task<IActionResult> Search(DateTime startDate, DateTime endDate, int? filterSite, int? length)
        {
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            var today = DateTime.Today;
            var minStartDate = today.AddDays(1);

            if (startDate < minStartDate)
            {
                ViewBag.Error = "Start date must be at least tomorrow.";
                return View("CreateReservation", new List<Site>());
            }

            if (endDate <= startDate)
            {
                ViewBag.Error = "End date must be after start date.";
                return View("CreateReservation", new List<Site>());
            }

            var isPCS = HttpContext.Session.GetInt32("IsPCSOrders") == 1;
            var stayLength = (endDate - startDate).Days;
            var isPeakSeason = startDate >= new DateTime(startDate.Year, 4, 1) && startDate <= new DateTime(startDate.Year, 10, 15);

            if (!isPCS && isPeakSeason && stayLength > 14)
            {
                ViewBag.Error = "Maximum stay is 14 days during peak season.";
                return View("CreateReservation", new List<Site>());
            }

            var query = _context.Sites.Include(s => s.SitePhotos).AsQueryable();

            query = query.Where(s => s.SiteTypeID == filterSite);

            if (filterSite == 1 && length.HasValue)
            {
                query = query.Where(s => s.MaxLengthFeet >= length.Value);
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
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            var site = await _context.Sites.FindAsync(siteId);
            if (site == null) return NotFound();

            var isPCS = HttpContext.Session.GetInt32("IsPCSOrders") == 1;
            ViewBag.IsPCSOrders = isPCS;

            var model = new CreateReservationViewModel
            {
                SiteID = siteId,
                SiteNumber = site.SiteNumber,
                StartDate = startDate == default ? DateTime.Today.AddDays(1) : startDate,
                EndDate = endDate == default ? DateTime.Today.AddDays(2) : endDate,
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

            var today = DateTime.Today;
            var minStartDate = today.AddDays(1);
            if (model.StartDate < minStartDate)
            {
                ModelState.AddModelError("", "Start date must be at least tomorrow.");
                return View(model);
            }

            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
                return View(model);
            }

            var isPCS = HttpContext.Session.GetInt32("IsPCSOrders") == 1;
            var stayLength = (model.EndDate - model.StartDate).Days;
            var isPeakSeason = model.StartDate >= new DateTime(model.StartDate.Year, 4, 1) && model.StartDate <= new DateTime(model.StartDate.Year, 10, 15);

            if (!isPCS && isPeakSeason && stayLength > 14)
            {
                ModelState.AddModelError("", "Maximum stay is 14 days during peak season.");
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            var customerEmail = customer.User?.Email ?? "";
            var customerPhone = customer.PhoneNumber ?? "";

            var site = await _context.Sites
                .Include(s => s.SiteType)
                    .ThenInclude(st => st.SiteTypePricings)
                .FirstOrDefaultAsync(s => s.SiteID == model.SiteID);

            if (site == null)
                return NotFound();

            int numberOfNights = (model.EndDate - model.StartDate).Days;

            var pricing = site.SiteType.SiteTypePricings
                .Where(p => p.StartDate <= model.StartDate && (p.EndDate == null || p.EndDate >= model.StartDate))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            decimal nightlyRate = pricing?.BasePrice ?? 0m;
            decimal totalAmount = nightlyRate * numberOfNights;

            HttpContext.Session.SetString("ReservationData", System.Text.Json.JsonSerializer.Serialize(new
            {
                SiteID = model.SiteID,
                SiteNumber = site.SiteNumber,
                CustomerID = customer.CustomerID,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TrailerLengthFeet = model.TrailerLengthFeet,
                NumberOfGuests = model.NumberOfGuests,
                NumberOfPets = model.NumberOfPets,
                SpecialRequests = model.SpecialRequests,
                TotalAmount = totalAmount,
                NumberOfNights = numberOfNights,
                PricePerNight = nightlyRate,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone
            }));

            return RedirectToAction("CreateReservationPayment", "Payments");
        }

        public async Task<IActionResult> MyReservations()
        {
            var redirect = RequireLogin();
            if (redirect != null)
                return redirect;

            await UpdateReservationStatuses();

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
                .OrderByDescending(r => r.EndDate)
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
                return RedirectToAction("Login", "Account");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return RedirectToAction("Login", "Account");

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

            var isPCS = HttpContext.Session.GetInt32("IsPCSOrders") == 1;
            var stayLength = (updatedReservation.EndDate - updatedReservation.StartDate).Days;
            var isPeakSeason = updatedReservation.StartDate >= new DateTime(updatedReservation.StartDate.Year, 4, 1) && updatedReservation.StartDate <= new DateTime(updatedReservation.StartDate.Year, 10, 15);

            if (!isPCS && isPeakSeason && stayLength > 14)
            {
                ModelState.AddModelError("", "Maximum stay is 14 days during peak season.");
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
                return RedirectToAction("Login", "Account");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserID == userId.Value);

            if (customer == null)
                return RedirectToAction("Login", "Account");

            var reservation = await _context.Reservations
                .Include(r => r.Invoice)
                    .ThenInclude(i => i.Payments)
                .FirstOrDefaultAsync(r => r.ReservationID == id && r.CustomerID == customer.CustomerID);

            if (reservation == null)
                return RedirectToAction("AccessDenied", "Account");

            var upcomingStatus = await _context.ReservationStatuses
                .FirstOrDefaultAsync(rs => rs.StatusName == "Upcoming");
            
            if (upcomingStatus == null || reservation.ReservationStatusID != upcomingStatus.ReservationStatusID)
            {
                TempData["ErrorMessage"] = "You can only cancel upcoming reservations.";
                return RedirectToAction("MyReservations");
            }

            var cancelledStatus = await _context.ReservationStatuses
                .FirstOrDefaultAsync(rs => rs.StatusName == "Cancelled");

            if (cancelledStatus == null)
                return NotFound();

            var cancelFee = await _context.Fees.FirstOrDefaultAsync(f => f.FeeName == "Cancellation Fee");
            decimal feeAmount = cancelFee?.DefaultAmount ?? 10m;

            var reservationFee = new ReservationFee
            {
                ReservationID = reservation.ReservationID,
                FeeID = cancelFee?.FeeID ?? 3,
                Amount = feeAmount
            };
            _context.ReservationFees.Add(reservationFee);

            // Set TotalAmount to JUST the cancellation fee (not original amount + fee!)
            // Customer already paid for nights, canceling means they don't stay, so refund them minus fee
            reservation.TotalAmount = feeAmount;

            var amountPaid = reservation.Invoice?.Payments?
                .Where(p => !p.IsRefund).Sum(p => p.Amount) ?? 0;

            // Calculate: Cancellation fee ($10) - Already Paid ($25) = -$15 (refund!)
            // Negative = customer is owed money
            var balanceDue = feeAmount - amountPaid;

            if (balanceDue < 0 && reservation.Invoice != null)
            {
                // Customer paid more than fee, give refund
                var refundAmount = Math.Abs(balanceDue);
                
                var refund = new Payment
                {
                    InvoiceID = reservation.Invoice.InvoiceID,
                    PaymentMethodID = 1,
                    Amount = -refundAmount,
                    PaymentDate = DateTime.UtcNow,
                    StripeTransactionID = "sim_refund_" + Guid.NewGuid().ToString("N")[..16],
                    PaymentStatus = "Refunded",
                    IsRefund = true
                };
                _context.Payments.Add(refund);
                reservation.BalanceDue = 0;
                reservation.Invoice.TotalAmount = feeAmount;
                reservation.Invoice.IsPaid = true;
                reservation.Invoice.DatePaid = DateTime.UtcNow;

                TempData["SuccessMessage"] = $"Reservation cancelled. Refund of {refundAmount.ToString("C")} will be processed.";
            }
            else
            {
                // Customer didn't pay enough (edge case), owes the difference
                reservation.BalanceDue = balanceDue;
                if (reservation.Invoice != null)
                {
                    reservation.Invoice.TotalAmount = feeAmount;
                    reservation.Invoice.IsPaid = false;
                }

                TempData["SuccessMessage"] = $"Reservation cancelled. A cancellation fee of {feeAmount.ToString("C")} is due.";
            }

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
