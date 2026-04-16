using SiteReservationSystem.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteReservationSystem.Web.ViewModels
{
    public class RegisterViewModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [NotMapped]
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public MilitaryAffiliation MilitaryAffiliation { get; set; }

        [Required]
        public DoDStatus DoDStatus { get; set; }
    }
}