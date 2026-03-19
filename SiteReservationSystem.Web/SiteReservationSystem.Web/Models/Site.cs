using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class Site
    {
        [Key]
        public int SiteID { get; set; }

        [Required]
        [StringLength(10)]
        public string SiteNumber { get; set; } = string.Empty;

        [Required]
        [ForeignKey("SiteType")]
        public int SiteTypeID { get; set; }

        [Required]
        public int MaxLengthFeet { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public SiteType SiteType { get; set; } = null!;
        public ICollection<SitePhoto> SitePhotos { get; set; } = new List<SitePhoto>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
