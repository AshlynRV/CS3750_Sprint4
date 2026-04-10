using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Data;
using SiteReservationSystem.Web.Models;
using SiteReservationSystem.Web.ViewModels;

namespace SiteReservationSystem.Web.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays payment/invoice information for a reservation.
        /// 
        /// @Customer flow:
        /// - No parameters: Shows list of customer's invoices
        /// - With parameters: Shows selected invoice details. Customers can only view their own invoices.
        ///     - Can pay invoice if balance due is positive.
        ///     - Can get a refund if balance due < 0
        /// - Access denied redirects to Account/AccessDenied, back button puts you back to last page
        /// 
        /// @Admin/Employee flow:
        /// - Shows invoice summary for any reservation
        /// - Invalid/non-existent parameters show error message, back button puts you back to last page
        /// </summary>
        /// <param name="reservationId">Reservation ID (optional)</param>
        /// <param name="invoiceId">Invoice ID (optional)</param>
        /// <returns>Payment view with invoice details</returns>
        public IActionResult Index(int reservationId = 0, int invoiceId = 0)
        {
            // Ensure user is logged in before accessing any payment information
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            // Determine user role from session to handle different flows
            var userRole = HttpContext.Session.GetString("UserRole");
            var isAdmin = userRole == "Admin" || userRole == "Employee";
            var isCustomer = userRole == "Customer";
            var loggedInCustomerId = HttpContext.Session.GetInt32("CustomerID");

            // Customer Flow: No parameters provided - show invoice list
            if (isCustomer && loggedInCustomerId.HasValue && reservationId == 0 && invoiceId == 0)
            {
                // Clear any previous error messages
                TempData.Remove("ErrorMessage");
                
                // Fetch all invoices for this customer with related data
                // Include Payments to calculate actual status based on payment history
                var invoices = _context.Invoices
                    .Include(i => i.Reservation)
                        .ThenInclude(r => r.Site)
                            .ThenInclude(s => s.SiteType)
                    .Include(i => i.Payments)
                    .Where(i => i.CustomerID == loggedInCustomerId.Value)
                    .OrderByDescending(i => i.InvoiceDate)
                    .ToList();

                // Build invoice list view model with calculated payment status
                // We calculate IsPaid/NeedsRefund from actual payments, not the database IsPaid flag
                // because totals can change after initial payment (fees added/removed, dates changed)
                var invoiceListViewModel = new InvoiceListViewModel
                {
                    Invoices = invoices.Select(i => 
                    {
                        // Calculate actual balance based on non-refund payments
                        var amountPaid = i.Payments?.Where(p => !p.IsRefund).Sum(p => p.Amount) ?? 0;
                        var balanceDue = i.TotalAmount - amountPaid;
                        
                        return new InvoiceItemViewModel
                        {
                            ReservationID = i.ReservationID,
                            InvoiceID = i.InvoiceID,
                            SiteNumber = i.Reservation.Site.SiteNumber,
                            SiteTypeName = i.Reservation.Site.SiteType.TypeName,
                            StartDate = i.Reservation.StartDate,
                            EndDate = i.Reservation.EndDate,
                            NumberOfNights = (int)(i.Reservation.EndDate - i.Reservation.StartDate).TotalDays,
                            TotalAmount = i.TotalAmount,
                            // IsPaid = true if balanceDue <= 0 (paid or overpaid)
                            IsPaid = balanceDue <= 0,
                            // NeedsRefund = true only if overpaid (balanceDue < 0)
                            NeedsRefund = balanceDue < 0,
                            InvoiceDate = i.InvoiceDate,
                        };
                    }).ToList(),
                };

                // Set ViewBag flags to control view rendering
                ViewBag.ShowInvoiceList = true;
                ViewBag.InvoiceList = invoiceListViewModel.Invoices;
                return View(new PaymentViewModel());
            }

            // Fetch Reservation: Get all related data for display
            var reservation = _context.Reservations
                .Include(r => r.Site)
                    .ThenInclude(s => s.SiteType)
                        .ThenInclude(st => st.SiteTypePricings)  // For price per night lookup
                .Include(r => r.Customer)
                    .ThenInclude(c => c.User)  // For customer email
                .Include(r => r.Invoice)
                    .ThenInclude(i => i!.Payments)  // For calculating actual balance
                .Include(r => r.ReservationFees)
                    .ThenInclude(rf => rf.Fee)  // For fee breakdown display
                .FirstOrDefault(r => r.ReservationID == reservationId);

            // Handle Invalid/Not Found: IE no params for admin, invalid IDs, or reservation not found
            if (reservation == null || reservation.Invoice == null)
            {
                if (isAdmin)
                {
                    // Admins see inline error, can use back button
                    ViewBag.ShowInvalidParamsPopup = true;
                    ViewBag.IsAdmin = isAdmin;
                    return View(new PaymentViewModel());
                }
                TempData["ErrorMessage"] = "Reservation or invoice not found.";
                return RedirectToAction("AccessDenied", "Account");
            }

            // Validate Invoice Matches Reservation
            if (reservation.Invoice.InvoiceID != invoiceId)
            {
                if (isAdmin)
                {
                    ViewBag.ShowInvalidParamsPopup = true;
                    ViewBag.IsAdmin = isAdmin;
                    return View(new PaymentViewModel());
                }
                TempData["ErrorMessage"] = "You do not have access to this invoice.";
                return RedirectToAction("AccessDenied", "Account");
            }

            // Ensure customers can only view their own invoices
            if (isCustomer)
            {
                if (!loggedInCustomerId.HasValue || reservation.CustomerID != loggedInCustomerId.Value)
                {
                    TempData["ErrorMessage"] = "You do not have access to this invoice.";
                    return RedirectToAction("AccessDenied", "Account");
                }
            }

            // Determine balance and payment state from actual payment data
            var numberOfNights = (int)(reservation.EndDate - reservation.StartDate).TotalDays;
            
            // Sum all fees from reservation-fee table
            var actualFees = reservation.ReservationFees.Sum(rf => rf.Amount);
            
            // Calculate actual amount paid (exclude refunds)
            var amountPaid = reservation.Invoice.Payments?.Where(p => !p.IsRefund).Sum(p => p.Amount) ?? 0;
            
            // BalanceDue = TotalAmount - AmountPaid (can be negative if overpaid)
            var actualBalanceDue = reservation.TotalAmount - amountPaid;
            
            // isPaidInFull = true when balance is 0 or negative
            var isPaidInFull = actualBalanceDue <= 0;

            // Get price per night from pricing table based on reservation start date
            // Filter by date range: StartDate must be >= pricing.StartDate and <= pricing.EndDate
            var pricing = reservation.Site.SiteType.SiteTypePricings
                .Where(p => p.StartDate <= reservation.StartDate && (p.EndDate == null || p.EndDate >= reservation.StartDate))
                .OrderByDescending(p => p.StartDate)  // Get most recent pricing
                .FirstOrDefault();
            var pricePerNight = pricing?.BasePrice ?? 0;

            // Pass data to view via ViewBag and ViewModel
            ViewBag.IsAdmin = isAdmin;
            ViewBag.PricePerNight = pricePerNight;

            // Determine if refund is available (overpaid)
            var needsRefund = actualBalanceDue < 0;

            // Build PaymentViewModel with all display data
            var viewModel = new PaymentViewModel
            {
                // Basic reservation info
                ReservationID = reservation.ReservationID,
                SiteNumber = reservation.Site.SiteNumber,
                SiteTypeName = reservation.Site.SiteType.TypeName,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                NumberOfNights = numberOfNights,
                
                // Financial data
                TotalAmount = reservation.TotalAmount,
                BaseAmount = reservation.BaseAmount,
                TotalFees = actualFees,
                AmountPaid = amountPaid,
                BalanceDue = actualBalanceDue,
                
                // Customer info for pre-filling form
                CustomerName = $"{reservation.Customer.FirstName} {reservation.Customer.LastName}",
                CustomerEmail = reservation.Customer.User.Email,
                CustomerPhone = reservation.Customer.PhoneNumber,
                
                // Payment state flags
                // CanPay = customer owns this invoice AND not paid in full AND not owed a refund
                CanPay = isCustomer && reservation.CustomerID == loggedInCustomerId && !isPaidInFull,
                // IsAlreadyPaid = paid in full AND not owed refund (show "paid in full" message)
                IsAlreadyPaid = isPaidInFull && !needsRefund,
                NeedsRefund = needsRefund,
                
                // Invoice reference
                InvoiceID = reservation.Invoice?.InvoiceID,
                DatePaid = reservation.Invoice?.DatePaid,
                
                // Fee breakdown for display
                Fees = reservation.ReservationFees.Select(rf => new FeeItemViewModel
                {
                    FeeName = rf.Fee.FeeName,
                    Amount = rf.Amount,
                }).ToList(),
            };

            return View(viewModel);
        }

        /// <summary>
        /// Processes a payment for a customer's invoice.
        /// Calculates balance due (TotalAmount - existing payments) and creates a new payment record.
        /// Updates invoice status to paid and syncs TotalAmount with reservation.
        /// </summary>
        /// <param name="model">Payment form data</param>
        /// <returns>Redirects to payment view with success/error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(PaymentViewModel model)
        {
            // Ensure user is logged in
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            // Fetch reservation with invoice and payment history
            var reservation = _context.Reservations
                .Include(r => r.Invoice)
                    .ThenInclude(i => i!.Payments)
                .FirstOrDefault(r => r.ReservationID == model.ReservationID);

            // Validate reservation exists
            if (reservation == null || reservation.Invoice == null)
            {
                TempData["ErrorMessage"] = "Invalid reservation or invoice not found.";
                return RedirectToAction("AccessDenied", "Account");
            }

            // Validate form fields
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
                return RedirectToAction("Index", new { reservationId = model.ReservationID, invoiceId = model.InvoiceID });
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            
            // Only process payment for customers
            if (userRole == "Customer")
            {
                // Calculate how much is still owed (TotalAmount - existing payments)
                var amountPaid = reservation.Invoice.Payments?.Where(p => !p.IsRefund).Sum(p => p.Amount) ?? 0;
                var balanceDue = reservation.TotalAmount - amountPaid;

                // Create payment record for the balance due amount
                var payment = new Payment
                {
                    InvoiceID = reservation.Invoice.InvoiceID,
                    PaymentMethodID = 1,  // Card payment
                    Amount = balanceDue,  // Pay only what's owed, not full total
                    PaymentDate = DateTime.UtcNow,
                    StripeTransactionID = "sim_" + Guid.NewGuid().ToString("N")[..16],  // Simulated Stripe ID
                    PaymentStatus = "Completed",
                    IsRefund = false,
                };

                _context.Payments.Add(payment);

                // Update invoice and reservation status
                reservation.Invoice.IsPaid = true;
                reservation.Invoice.DatePaid = DateTime.UtcNow;
                reservation.Invoice.TotalAmount = reservation.TotalAmount;  // Sync with reservation
                reservation.BalanceDue = 0;  // Fully paid

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Payment Successful! Confirmation email sent.";
            }

            // Redirect back to payment page to show updated status
            return RedirectToAction("Index", new { reservationId = model.ReservationID, invoiceId = model.InvoiceID });
        }

        /// <summary>
        /// Processes a refund for a customer when they've overpaid.
        /// Adjusts the original payment and creates a refund record
        /// Marks invoice as paid and sets balance due to 0.
        /// </summary>
        /// <param name="model">Payment form data</param>
        /// <returns>Redirects to payment view with refund success message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessRefund(PaymentViewModel model)
        {
            // Ensure user is logged in
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            // Fetch reservation with invoice and payment history
            var reservation = _context.Reservations
                .Include(r => r.Invoice)
                    .ThenInclude(i => i!.Payments)
                .FirstOrDefault(r => r.ReservationID == model.ReservationID);

            // Validate reservation exists
            if (reservation == null || reservation.Invoice == null)
            {
                TempData["ErrorMessage"] = "Invalid reservation or invoice not found.";
                return RedirectToAction("AccessDenied", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            var loggedInCustomerId = HttpContext.Session.GetInt32("CustomerID");

            // Only process refunds for customers
            if (userRole == "Customer")
            {
                // Verify customer owns this invoice
                if (!loggedInCustomerId.HasValue || reservation.CustomerID != loggedInCustomerId.Value)
                {
                    TempData["ErrorMessage"] = "You do not have access to this invoice.";
                    return RedirectToAction("AccessDenied", "Account");
                }

                // Calculate refund amount (amountPaid - totalAmount = overpayment)
                var amountPaid = reservation.Invoice.Payments?.Where(p => !p.IsRefund).Sum(p => p.Amount) ?? 0;
                var refundAmount = amountPaid - reservation.TotalAmount;

                // Only process if there's actually an overpayment
                if (refundAmount > 0)
                {
                    // Find the original payment to adjust its amount
                    // This keeps payment records accurate (original amount - refund = actual paid)
                    var originalPayment = reservation.Invoice.Payments?
                        .Where(p => !p.IsRefund && p.Amount > 0)
                        .OrderByDescending(p => p.PaymentDate)
                        .FirstOrDefault();

                    if (originalPayment != null)
                    {
                        // Reduce original payment by refund amount
                        originalPayment.Amount -= refundAmount;
                    }

                    // Create refund record (negative amount = money out)
                    var refund = new Payment
                    {
                        InvoiceID = reservation.Invoice.InvoiceID,
                        PaymentMethodID = 1,  // Card payment
                        Amount = -refundAmount,  // Negative = refund
                        PaymentDate = DateTime.UtcNow,
                        StripeTransactionID = "sim_" + Guid.NewGuid().ToString("N")[..16],
                        PaymentStatus = "Refunded",
                        IsRefund = true,  // Flag for filtering in calculations
                    };

                    _context.Payments.Add(refund);

                    // Update reservation and invoice to reflect zero balance
                    reservation.BalanceDue = 0;
                    reservation.Invoice.IsPaid = true;  // "Paid" because no balance owed

                    _context.SaveChanges();
                }

                TempData["SuccessMessage"] = $"Refund Successful! We've refunded ${Math.Abs(refundAmount):F2} to your card.";
            }

            // Redirect back to payment page to show updated status
            return RedirectToAction("Index", new { reservationId = model.ReservationID, invoiceId = model.InvoiceID });
        }
    }
}
