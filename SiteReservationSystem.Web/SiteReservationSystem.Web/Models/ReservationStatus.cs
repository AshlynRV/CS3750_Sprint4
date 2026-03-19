using System.ComponentModel.DataAnnotations;

namespace SiteReservationSystem.Web.Models
{
    public class ReservationStatus
    {
        [Key]
        public int ReservationStatusID { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
