// Models/SiteType.cs
namespace SiteReservationSystem.Web.Models
{
    public class SiteType
    {
        public int SiteTypeId { get; set; }
        public string Name { get; set; }

        public List<Site> Sites { get; set; }
        public List<SiteTypePrice> SiteTypePrices { get; set; }
    }
}