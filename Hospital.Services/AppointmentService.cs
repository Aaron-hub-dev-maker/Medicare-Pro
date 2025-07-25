using Hospital.Models;
using Hospitals.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Services
{
    /// <summary>
    /// Service for appointment-related business logic.
    /// </summary>
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Appointment> GetAllAppointments(string searchTerm = "", string doctorFilter = "", string patientFilter = "", string typeFilter = "")
        {
            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a =>
                    a.Number.Contains(searchTerm) ||
                    a.Description.Contains(searchTerm) ||
                    a.City.Contains(searchTerm) ||
                    (a.Doctor != null && a.Doctor.Name.Contains(searchTerm)) ||
                    (a.Patient != null && a.Patient.Name.Contains(searchTerm))
                );
            }
            if (!string.IsNullOrEmpty(doctorFilter))
                query = query.Where(a => a.DoctorId == doctorFilter);
            if (!string.IsNullOrEmpty(patientFilter))
                query = query.Where(a => a.PatientId == patientFilter);
            if (!string.IsNullOrEmpty(typeFilter))
                query = query.Where(a => a.Type == typeFilter);

            return query.OrderByDescending(a => a.CreatedDate).ToList();
        }

        public async Task<bool> BookAppointmentAsync(string doctorId, string patientId, string description, string appointmentType = "General")
        {
            if (string.IsNullOrEmpty(doctorId) || string.IsNullOrEmpty(patientId))
                return false;

            var lastAppointment = _context.Appointments.OrderByDescending(a => a.Id).FirstOrDefault();
            int nextNumber = 1;
            if (lastAppointment != null && !string.IsNullOrEmpty(lastAppointment.Number) && lastAppointment.Number.StartsWith("APT-"))
            {
                var numberPart = lastAppointment.Number.Substring(4);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            // Fetch patient to get city
            var patient = _context.ApplicationUsers.FirstOrDefault(u => u.Id == patientId);
            var city = patient?.City ?? "Unknown";

            var appointment = new Appointment
            {
                Number = $"APT-{nextNumber:D3}",
                CreatedDate = DateTime.Now,
                DoctorId = doctorId,
                PatientId = patientId,
                Description = description,
                Type = appointmentType,
                City = city // <-- Set city here
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            var lastAppointment = _context.Appointments.OrderByDescending(a => a.Id).FirstOrDefault();
            int nextNumber = 1;
            if (lastAppointment != null && !string.IsNullOrEmpty(lastAppointment.Number) && lastAppointment.Number.StartsWith("APT-"))
            {
                var numberPart = lastAppointment.Number.Substring(4);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            appointment.Number = $"APT-{nextNumber:D3}";
            appointment.CreatedDate = DateTime.Now;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> EditAppointmentAsync(int id, Appointment appointment)
        {
            var existing = await _context.Appointments.FindAsync(id);
            if (existing == null) return null;
            existing.Type = appointment.Type;
            existing.Description = appointment.Description;
            existing.DoctorId = appointment.DoctorId;
            existing.PatientId = appointment.PatientId;
            existing.City = appointment.City;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public Appointment GetAppointmentById(int id)
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<string> GetAppointmentTypes()
        {
            return _context.Appointments.Select(a => a.Type).Distinct().ToList();
        }

        public IEnumerable<ApplicationUser> GetDoctors()
        {
            return _context.ApplicationUsers.Where(u => u.IsDoctor).ToList();
        }

        public IEnumerable<ApplicationUser> GetPatients()
        {
            return _context.ApplicationUsers.Where(u => !u.IsDoctor && u.UserName != "admin").ToList();
        }

        public IEnumerable<DateTime> GetAvailableSlots(string doctorId, DateTime date)
        {
            // Use Timing.Date instead of ScheduleDate
            var timings = _context.Timings.Where(t => t.DoctorId == doctorId && t.Date.Date == date.Date).ToList();
            var slots = new List<DateTime>();
            foreach (var timing in timings)
            {
                // Morning shift
                for (int hour = timing.MorningShiftStartTime; hour < timing.MorningShiftEndTime; hour++)
                {
                    slots.Add(new DateTime(date.Year, date.Month, date.Day, hour, 0, 0));
                }
                // Evening shift
                for (int hour = timing.AfternoonShiftStartTime; hour < timing.AfternoonShiftEndTime; hour++)
                {
                    slots.Add(new DateTime(date.Year, date.Month, date.Day, hour, 0, 0));
                }
            }
            // Remove slots that are already booked
            var booked = _context.Appointments.Where(a => a.DoctorId == doctorId && a.CreatedDate.Date == date.Date)
                .Select(a => a.CreatedDate.Hour).ToList();
            return slots.Where(s => !booked.Contains(s.Hour)).ToList();
        }

        public bool IsSlotAvailable(string doctorId, DateTime slot)
        {
            return !_context.Appointments.Any(a => a.DoctorId == doctorId && a.CreatedDate == slot);
        }
    }
} 