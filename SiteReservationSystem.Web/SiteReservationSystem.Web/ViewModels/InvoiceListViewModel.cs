namespace SiteReservationSystem.Web.ViewModels
{
    public class InvoiceListViewModel
    {
        public List<InvoiceItemViewModel> Invoices { get; set; } = new();
    }

    public class InvoiceItemViewModel
    {
        public int ReservationID { get; set; }
        public int InvoiceID { get; set; }
        public string SiteNumber { get; set; } = string.Empty;
        public string SiteTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfNights { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public bool NeedsRefund { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
