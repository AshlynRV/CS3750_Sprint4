using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class SitePhoto
    {
        [Key]
        public int PhotoID { get; set; }

        [Required]
        [ForeignKey("Site")]
        public int SiteID { get; set; }

        [Required]
        [StringLength(500)]
        public string PhotoURL { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Caption { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public DateTime DateUploaded { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Site Site { get; set; } = null!;
    }
}
