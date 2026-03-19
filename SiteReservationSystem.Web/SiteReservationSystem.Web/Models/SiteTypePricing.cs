using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class SiteTypePricing
    {
        [Key]
        public int PricingID { get; set; }

        [Required]
        [ForeignKey("SiteType")]
        public int SiteTypeID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; } // NULL = current/active price

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public SiteType SiteType { get; set; } = null!;
    }
}
