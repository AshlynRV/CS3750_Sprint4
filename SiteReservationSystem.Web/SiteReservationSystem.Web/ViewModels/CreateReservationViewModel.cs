using System.ComponentModel.DataAnnotations;

namespace SiteReservationSystem.Web.ViewModels
{
    public class CreateReservationViewModel
    {
        public int SiteID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? TrailerLengthFeet { get; set; }

        public int NumberOfGuests { get; set; }

        public int NumberOfPets { get; set; }

        public string? SpecialRequests { get; set; }
    }
}