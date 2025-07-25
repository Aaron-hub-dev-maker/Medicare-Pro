using Hospitals.Repositories.Interfaces;
using Hospital.Models;
using System.Linq;

namespace Hospital.Services
{
    /// <summary>
    /// Service for dashboard/statistics aggregation logic.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public object GetDashboardData()
        {
            var totalDoctors = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(x => x.IsDoctor == true).Count();
            var totalPatients = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(x => x.IsDoctor == false && x.UserName != "admin").Count();
            var totalHospitals = _unitOfWork.GenericRepository<HospitalInfo>().GetAll().Count();
            var totalRooms = _unitOfWork.GenericRepository<Hospital.Models.Room>().GetAll().Count();
            var availableRooms = _unitOfWork.GenericRepository<Hospital.Models.Room>()
                .GetAll(x => x.Status == "Available").Count();
            var totalAppointments = _unitOfWork.GenericRepository<Appointment>().GetAll().Count();
            var recentAppointments = _unitOfWork.GenericRepository<Appointment>()
                .GetAll()
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .ToList();
            var totalContacts = _unitOfWork.GenericRepository<Contact>().GetAll().Count();
            var recentDoctors = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(x => x.IsDoctor == true)
                .OrderByDescending(x => x.Id)
                .Take(3)
                .ToList();
            var recentPatients = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(x => x.IsDoctor == false && x.UserName != "admin")
                .OrderByDescending(x => x.Id)
                .Take(3)
                .ToList();
            return new
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                TotalHospitals = totalHospitals,
                TotalRooms = totalRooms,
                AvailableRooms = availableRooms,
                TotalAppointments = totalAppointments,
                TotalContacts = totalContacts,
                RecentAppointments = recentAppointments,
                RecentDoctors = recentDoctors,
                RecentPatients = recentPatients
            };
        }
    }
} 