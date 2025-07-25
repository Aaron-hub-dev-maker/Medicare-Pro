using Hospital.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the admin dashboard.
    /// </summary>
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Dashboard()
        {
            var dashboardData = _dashboardService.GetDashboardData();
            return View(dashboardData);
        }
    }
} 