// Models/SiteTypePrice.cs
namespace SiteReservationSystem.Web.Models
{
    public class SiteTypePrice
    {
        public int SiteTypePriceId { get; set; }
        public int SiteTypeId { get; set; }
        public decimal Price { get; set; }

        public SiteType SiteType { get; set; }
    }
}