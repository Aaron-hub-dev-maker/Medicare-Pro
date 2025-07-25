using Hospital.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Web.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class AppointmentsController : Controller
    {
        private readonly IApplicationUserService _userService;
        private readonly IAppointmentService _appointmentService;
        public AppointmentsController(IApplicationUserService userService, IAppointmentService appointmentService)
        {
            _userService = userService;
            _appointmentService = appointmentService;
        }

        public IActionResult Index()
        {
            // Get the logged-in patient's ID
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointments = _appointmentService.GetAllAppointments()
                .Where(a => a.PatientId == patientId)
                .ToList();
            return View(appointments);
        }

        public IActionResult BookAppointment(int pageNumber = 1, int pageSize = 20)
        {
            var doctors = _userService.GetAllDoctor(pageNumber, pageSize);
            return View(doctors);
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentBookingDto dto)
        {
            var patientId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var result = await _appointmentService.BookAppointmentAsync(dto.DoctorId, patientId, dto.Description, dto.AppointmentType);
            if (result)
                return Json(new { success = true });
            return Json(new { success = false, message = "Failed to book appointment." });
        }

        public class AppointmentBookingDto
        {
            public string DoctorId { get; set; }
            public string Description { get; set; }
            public string AppointmentType { get; set; }
        }

        [HttpGet]
        public JsonResult GetAvailableSlots(string doctorId, DateTime date)
        {
            var slots = _appointmentService.GetAvailableSlots(doctorId, date);
            return Json(slots.Select(s => s.ToString("yyyy-MM-dd HH:mm")));
        }
    }
} 