using System.ComponentModel.DataAnnotations;

namespace SiteReservationSystem.Web.Models
{
    public class SiteType
    {
        [Key]
        public int SiteTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Site> Sites { get; set; } = new List<Site>();
        public ICollection<SiteTypePricing> SiteTypePricings { get; set; } = new List<SiteTypePricing>();
    }
}
