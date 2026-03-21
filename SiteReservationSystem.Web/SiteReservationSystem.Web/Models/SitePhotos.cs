// Models/SitePhoto.cs
namespace SiteReservationSystem.Web.Models
{
    public class SitePhoto
    {
        public int SitePhotoId { get; set; }
        public int SiteId { get; set; }
        public string Url { get; set; }

        public Site Site { get; set; }
    }
}
