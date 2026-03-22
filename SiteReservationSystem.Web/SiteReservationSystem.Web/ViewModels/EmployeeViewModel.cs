using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; } // Added
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public AccessPermissions AccessPermissions { get; set; } = AccessPermissions.None;
        public bool IsLockedOut { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();
    }
}