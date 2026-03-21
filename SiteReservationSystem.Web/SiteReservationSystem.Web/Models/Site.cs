// Models/Site.cs
namespace SiteReservationSystem.Web.Models
{
    public class Site
    {
        public int SiteId { get; set; }
        public string Name { get; set; }
        public int SiteTypeId { get; set; }

        public SiteType SiteType { get; set; }
        public List<SitePhoto> SitePhotos { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
