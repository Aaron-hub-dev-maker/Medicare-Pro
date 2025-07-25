using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Hospitals.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpPost]
        public async Task<IActionResult> BookAppointment(string doctorId, string description)
        {
            var patient = await _userManager.GetUserAsync(User);

            if (patient == null || string.IsNullOrEmpty(doctorId))
            {
                return RedirectToAction("AllDoctors", "Users", new { area = "Admin" });
            }

            var lastAppointment = _context.Appointments
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            int nextNumber = 1;
            if (lastAppointment != null && !string.IsNullOrEmpty(lastAppointment.Number) && lastAppointment.Number.StartsWith("APT-"))
            {
                var numberPart = lastAppointment.Number.Substring(4);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string newAppointmentNumber = $"APT-{nextNumber.ToString("D3")}";

            var appointment = new Appointment
            {
                Number = newAppointmentNumber,
                Type = "General",
                City = patient.City ?? "Unknown",
                CreatedDate = DateTime.Now,
                Description = string.IsNullOrEmpty(description) ? "Booked via doctor list" : description, // Use input or default
                DoctorId = doctorId,
                PatientId = patient.Id
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment booked successfully!";
            //return RedirectToAction("AllAppointments");
            return RedirectToAction("AllDoctors", "Users", new { area = "Admin" });

        }

        [HttpGet]
        public IActionResult BookAppointment(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
            {
                return RedirectToAction("AllDoctors", "Users", new { area = "Admin" });
            }

            ViewBag.DoctorId = doctorId;
            return View();
        }






        public IActionResult AllAppointments()
        {
            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                    //.ThenInclude(d => d.Department)
                     //   .ThenInclude(dep => dep.Hospital)
                .Include(a => a.Patient)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();

            return View(appointments);
        }

    }
}
