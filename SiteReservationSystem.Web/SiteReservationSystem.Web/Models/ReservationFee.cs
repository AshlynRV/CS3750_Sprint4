using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class ReservationFee
    {
        [Key]
        public int ReservationFeeID { get; set; }

        [Required]
        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }

        [Required]
        [ForeignKey("Fee")]
        public int FeeID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime DateApplied { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? Notes { get; set; }

        // Navigation properties
        public Reservation Reservation { get; set; } = null!;
        public Fee Fee { get; set; } = null!;
    }
}
