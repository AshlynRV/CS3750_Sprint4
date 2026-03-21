// Models/Reservation.cs
namespace SiteReservationSystem.Web.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int SiteId { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Site Site { get; set; }
        public Customer Customer { get; set; }
    }
}