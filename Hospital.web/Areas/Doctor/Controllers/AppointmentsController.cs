using Hospital.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public IActionResult Index()
        {
            // Get the logged-in doctor's ID
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = _appointmentService.GetAllAppointments()
                .Where(a => a.DoctorId == doctorId)
                .ToList();
            return View(appointments);
        }
    }
} 