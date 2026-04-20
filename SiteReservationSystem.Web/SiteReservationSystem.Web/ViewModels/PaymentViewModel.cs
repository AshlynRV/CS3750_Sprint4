namespace SiteReservationSystem.Web.ViewModels
{
    public class PaymentViewModel
    {
        public int ReservationID { get; set; }
        public string SiteNumber { get; set; } = string.Empty;
        public string SiteTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal TotalFees { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public bool CanPay { get; set; }
        public bool IsAlreadyPaid { get; set; }
        public bool NeedsRefund { get; set; }
        public int? InvoiceID { get; set; }
        public DateTime? DatePaid { get; set; }
        public List<FeeItemViewModel> Fees { get; set; } = new();
    }

    public class FeeItemViewModel
    {
        public string FeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class ReservationDataViewModel
    {
        public int SiteID { get; set; }
        public string? SiteNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? TrailerLengthFeet { get; set; }
        public int NumberOfGuests { get; set; }
        public int NumberOfPets { get; set; }
        public string? SpecialRequests { get; set; }
        public decimal TotalAmount { get; set; }
        public int NumberOfNights { get; set; }
        public decimal PricePerNight { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
    }
}
