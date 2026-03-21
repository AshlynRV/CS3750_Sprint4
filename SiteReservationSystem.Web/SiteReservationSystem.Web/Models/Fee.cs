// Models/Fee.cs
namespace SiteReservationSystem.Web.Models
{
    public class Fee
    {
        public int FeeId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}