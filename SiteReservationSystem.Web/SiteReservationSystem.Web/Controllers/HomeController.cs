using Microsoft.AspNetCore.Mvc;
using SiteReservationSystem.Web.Models;
using System.Diagnostics;

namespace SiteReservationSystem.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            // Redirect to login page if user is not logged in. This is a function in BaseController.cs
            var redirect = RequireLogin();
            if (redirect != null)
                // Redirects to login page
                return redirect;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
