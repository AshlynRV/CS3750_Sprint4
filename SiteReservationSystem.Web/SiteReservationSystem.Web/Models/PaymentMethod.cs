using System.ComponentModel.DataAnnotations;

namespace SiteReservationSystem.Web.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodID { get; set; }

        [Required]
        [StringLength(50)]
        public string MethodName { get; set; } = string.Empty;

        public bool RequiresOnlineProcessing { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
