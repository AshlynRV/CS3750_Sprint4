using Microsoft.AspNetCore.Mvc;

namespace SiteReservationSystem.Web.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
