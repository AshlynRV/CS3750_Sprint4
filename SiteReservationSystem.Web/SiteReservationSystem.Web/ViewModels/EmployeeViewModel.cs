using SiteReservationSystem.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SiteReservationSystem.Web.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Password { get; set; } = string.Empty;

        public bool IsLockedOut { get; set; }

        // Change this from int to enum
        public AccessPermissions AccessPermissions { get; set; } = AccessPermissions.None;

        // For checkbox selections
        public List<AccessPermissions> SelectedPermissions { get; set; } = new List<AccessPermissions>();
    }
}