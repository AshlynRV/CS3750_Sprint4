using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceID { get; set; }

        [Required]
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalFees { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public DateTime DueDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime? DatePaid { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public Reservation Reservation { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
