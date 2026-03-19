using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        [ForeignKey("Invoice")]
        public int InvoiceID { get; set; }

        [Required]
        [ForeignKey("PaymentMethod")]
        public int PaymentMethodID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? StripeTransactionID { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending";

        [ForeignKey("Employee")]
        public int? ProcessedByEmployeeID { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsRefund { get; set; } = false;

        // Navigation properties
        public Invoice Invoice { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; } = null!;
        public Employee? ProcessedByEmployee { get; set; }
    }
}
