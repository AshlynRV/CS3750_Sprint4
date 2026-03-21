using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.Models
{
    public class Fee
    {
        [Key]
        public int FeeID { get; set; }

        [Required]
        [StringLength(100)]
        public string FeeName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DefaultAmount { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<ReservationFee> ReservationFees { get; set; } = new List<ReservationFee>();
    }
}
