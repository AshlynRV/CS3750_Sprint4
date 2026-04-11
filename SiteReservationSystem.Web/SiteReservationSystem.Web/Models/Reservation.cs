using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerID { get; set; }

        [Required]
        [ForeignKey("Site")]
        public int SiteID { get; set; }

        [Required]
        [ForeignKey("ReservationStatus")]
        public int ReservationStatusID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int? TrailerLengthFeet { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BalanceDue { get; set; }

        public DateTime ScheduledCheckInTime { get; set; }

        public DateTime? ActualCheckInTime { get; set; }

        public DateTime ScheduledCheckOutTime { get; set; }

        public DateTime? ActualCheckOutTime { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public Site Site { get; set; } = null!;
        public ReservationStatus ReservationStatus { get; set; } = null!;
        public ICollection<ReservationFee> ReservationFees { get; set; } = new List<ReservationFee>();
        public Invoice? Invoice { get; set; }

        public int NumberOfGuests { get; set; }

        public int NumberOfPets { get; set; }

        [StringLength(500)]
        public string? SpecialRequests { get; set; }
    }
}
