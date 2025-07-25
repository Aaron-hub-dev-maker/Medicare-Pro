using System.Diagnostics;
using Hospital.web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //return RedirectToAction("Index", "Home", new { area = "Patient" });
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AboutUs() => View();
        public IActionResult OurServices() => View();
        public IActionResult FindDoctor() => View();
        public IActionResult ContactUs() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
