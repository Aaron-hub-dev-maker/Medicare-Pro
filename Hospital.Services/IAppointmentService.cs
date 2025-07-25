using Hospital.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Hospital.Services
{
    /// <summary>
    /// Service interface for appointment-related business logic.
    /// </summary>
    public interface IAppointmentService
    {
        IEnumerable<Appointment> GetAllAppointments(string searchTerm = "", string doctorFilter = "", string patientFilter = "", string typeFilter = "");
        Task<bool> BookAppointmentAsync(string doctorId, string patientId, string description, string appointmentType = "General");
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task<Appointment> EditAppointmentAsync(int id, Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        Appointment GetAppointmentById(int id);
        IEnumerable<string> GetAppointmentTypes();
        IEnumerable<ApplicationUser> GetDoctors();
        IEnumerable<ApplicationUser> GetPatients();
        IEnumerable<DateTime> GetAvailableSlots(string doctorId, DateTime date);
        bool IsSlotAvailable(string doctorId, DateTime slot);
    }
} 