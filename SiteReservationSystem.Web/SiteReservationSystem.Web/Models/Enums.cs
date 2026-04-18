using System.ComponentModel.DataAnnotations;

namespace SiteReservationSystem.Web.Models
{
    public enum UserRole
    {
        Customer,
        Employee,
        Admin
    }

    public enum MilitaryAffiliation
    {
        [Display(Name = "Air Force")]
        AIR_FORCE,
        [Display(Name = "Army")]
        ARMY,
        [Display(Name = "Navy")]
        NAVY,
        [Display(Name = "Coast Guard")]
        COAST_GUARD,
        [Display(Name = "Marines")]
        MARINES,
        [Display(Name = "DoD Civilian")]
        DOD_CIVILIAN
    }

    public enum DoDStatus
    {
        [Display(Name = "Active Duty")]
        ACTIVE_DUTY,
        [Display(Name = "Reservist")]
        RESERVIST,
        [Display(Name = "Retired")]
        RETIRED,
        [Display(Name = "PCS Orders")]
        PCS_ORDERS  // Exempt from 14-day peak season limit
    }

    [Flags]
    public enum AccessPermissions
    {
        None = 0,
        ManageSites = 1,
        ManageSiteTypes = 2,
        ManageFees = 4,
        ManageReservations = 8,
        ViewReports = 16,
        ManageEmployees = 32
    }
}
