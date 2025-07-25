using Hospital.Models;
using Hospital.ViewModels;
using Hospitals.Repositories.Interfaces;
using Hospitals.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PagedResult<ApplicationUserViewModel> GetAll(int PageNumber, int PageSize)
        {
            int totalCount;
            List<ApplicationUserViewModel> vmList = new List<ApplicationUserViewModel>();

            try
            {
                int ExcludeRecords = (PageSize * PageNumber) - PageSize;

                var modelList = _unitOfWork.GenericRepository<ApplicationUser>()
                    .GetAll()
                    .OrderBy(x => x.Name)
                    .Skip(ExcludeRecords)
                    .Take(PageSize)
                    .ToList();

                totalCount = _unitOfWork.GenericRepository<ApplicationUser>()
                    .GetAll()
                    .Count();

                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }

            return new PagedResult<ApplicationUserViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
        }

        public PagedResult<ApplicationUserViewModel> GetAllDoctor(int PageNumber, int PageSize)
        {
            int totalCount;
            List<ApplicationUserViewModel> vmList = new List<ApplicationUserViewModel>();

            try
            {
                int ExcludeRecords = (PageSize * PageNumber) - PageSize;

                var modelList = _unitOfWork.GenericRepository<ApplicationUser>()
                    .Table  // IQueryable for Include support
                    .Where(x => x.IsDoctor == true)
                    .OrderBy(x => x.Name)
                    .Skip(ExcludeRecords)
                    .Take(PageSize)
                    .ToList();


                totalCount = _unitOfWork.GenericRepository<ApplicationUser>()
                    .GetAll(x => x.IsDoctor == true)
                    .Count();

                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }

            return new PagedResult<ApplicationUserViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = PageNumber,
                PageSize = PageSize
            };
        }

        public PagedResult<ApplicationUserViewModel> GetAllPatient(int PageNumber, int PageSize)
        {
            throw new NotImplementedException();
        }

        public PagedResult<ApplicationUserViewModel> SearchDoctor(int PageNumber, int PageSize, string Speciality = null)
        {
            throw new NotImplementedException();
        }

        public ApplicationUserViewModel GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var user = _unitOfWork.GenericRepository<ApplicationUser>().GetById(id);
            if (user == null)
                return null;

            var hospitalList = GetHospitals(); // ✅ Fetch hospital list
            return new ApplicationUserViewModel(user, hospitalList);
            //return new ApplicationUserViewModel(user);
        }

        public bool Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            var user = _unitOfWork.GenericRepository<ApplicationUser>().GetById(id);
            if (user == null)
                return false;

            try
            {
                // First, delete related appointments where this user is the doctor
                var appointments = _unitOfWork.GenericRepository<Appointment>()
                    .GetAll(a => a.DoctorId == id).ToList();
                if (appointments.Any())
                {
                    foreach (var appointment in appointments)
                    {
                        _unitOfWork.GenericRepository<Appointment>().Delete(appointment);
                    }
                }

                // Delete related timings
                var timings = _unitOfWork.GenericRepository<Timing>()
                    .GetAll(t => t.DoctorId == id).ToList();
                if (timings.Any())
                {
                    foreach (var timing in timings)
                    {
                        _unitOfWork.GenericRepository<Timing>().Delete(timing);
                    }
                }

                // Delete related patient reports where this user is the doctor
                var patientReports = _unitOfWork.GenericRepository<PatientReport>()
                    .GetAll(pr => pr.DoctorId == id).ToList();
                if (patientReports.Any())
                {
                    foreach (var report in patientReports)
                    {
                        _unitOfWork.GenericRepository<PatientReport>().Delete(report);
                    }
                }

                // Now delete the user
                _unitOfWork.GenericRepository<ApplicationUser>().Delete(user);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error deleting user {id}: {ex.Message}");
                return false;
            }
        }

        private List<ApplicationUserViewModel> ConvertModelToViewModelList(List<ApplicationUser> modelList)
        {
            var hospitalList = GetHospitals();
            return modelList.Select(x => new ApplicationUserViewModel(x, hospitalList)).ToList();
        }

        public IEnumerable<HospitalInfo> GetHospitals()
        {
            return _unitOfWork.GenericRepository<HospitalInfo>().GetAll();
        }

        /*
        // ✅ NEW: Get list of departments for dropdown
        public IEnumerable<SelectListItem> GetDepartments()
        {
            return _unitOfWork.GenericRepository<Department>()
                .GetAll()
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
        }
        */
        // ✅ NEW: Get list of hospitals for dropdown
        /*public IEnumerable<SelectListItem> GetHospitals()
        {
            return _unitOfWork.GenericRepository<HospitalInfo>()
                .GetAll()
                .Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                }).ToList();
        }*/

        // ✅ NEW: Get a single doctor with dropdown data
        public ApplicationUser GetDoctorById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var user = _unitOfWork.GenericRepository<ApplicationUser>()
                .Table // ✅ returns IQueryable<ApplicationUser>
                //.Include(u => u.Department)
                 //   .ThenInclude(d => d.Hospital)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return null;

            var hospitalList = GetHospitals(); // ✅ Fetch hospital list
            //var vm = new ApplicationUserViewModel(user, hospitalList);
            //var vm = new ApplicationUserViewModel(user);
            /*{
                Departments = GetDepartments(),
                Hospitals = GetHospitals()

            };*/

            return user;
        }


       

        // ✅ original one
        public bool UpdateDoctor(ApplicationUserViewModel viewModel)
        {
            if (viewModel.HospitalInfoId == null || viewModel.HospitalInfoId == 0)
                return false;
            var user = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetById(viewModel.Id);

            if (user == null)
                return false;

            user.Name = viewModel.Name;
            user.UserName = viewModel.UserName;
            user.Email = viewModel.Email;
            user.City = viewModel.City;
            user.Gender = viewModel.Gender;
            user.Specialist = viewModel.Specialist;
            user.HospitalInfoId = viewModel.HospitalInfoId;
            //user.DepartmentId = model.DepartmentId;

            _unitOfWork.GenericRepository<ApplicationUser>().Update(user);
            _unitOfWork.Save();
            // Debug logging
            var hospitalList = GetHospitals();
            var resolvedHospital = hospitalList.FirstOrDefault(h => h.Id == viewModel.HospitalInfoId);
            System.Diagnostics.Debug.WriteLine($"[UpdateDoctor] DoctorId: {user.Id}, HospitalInfoId: {viewModel.HospitalInfoId}, ResolvedHospital: {resolvedHospital?.Name}");
            return true;
        }

        public bool UpdateDoctor(ApplicationUser user)
        {
            var existingUser = _unitOfWork.GenericRepository<ApplicationUser>()
                                  .GetById(user.Id);

            if (existingUser == null)
                return false;

            existingUser.Name = user.Name;
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.City = user.City;
            existingUser.Gender = user.Gender;
            existingUser.Specialist = user.Specialist;
            existingUser.HospitalInfoId = user.HospitalInfoId;

            _unitOfWork.GenericRepository<ApplicationUser>().Update(existingUser);
            _unitOfWork.Save();

            return true;
        }
    }
}
