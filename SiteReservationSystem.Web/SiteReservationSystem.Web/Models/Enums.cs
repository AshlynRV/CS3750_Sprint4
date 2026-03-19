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
        AIR_FORCE,
        ARMY,
        NAVY,
        COAST_GUARD,
        MARINES,
        DOD_CIVILIAN
    }

    public enum DoDStatus
    {
        ACTIVE_DUTY,
        RESERVIST,
        RETIRED,
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
