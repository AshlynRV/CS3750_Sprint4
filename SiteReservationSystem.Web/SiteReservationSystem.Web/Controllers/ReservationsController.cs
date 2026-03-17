using Microsoft.AspNetCore.Mvc;

namespace SiteReservationSystem.Web.Controllers
{
    public class ReservationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
