using Hospital.Models;
using Hospital.ViewModels;
using Hospitals.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Hospital.Services
{
    public interface IApplicationUserService
    {
        bool Delete(string id);

        PagedResult<ApplicationUserViewModel> GetAll(int PageNumber, int PageSize);

        PagedResult<ApplicationUserViewModel> GetAllDoctor(int PageNumber, int PageSize);

        PagedResult<ApplicationUserViewModel> GetAllPatient(int PageNumber, int PageSize);

        ApplicationUserViewModel? GetById(string id);

        // ✅ Return proper dropdown types
        /*
        IEnumerable<SelectListItem> GetDepartments();
        IEnumerable<SelectListItem> GetHospitals();
        */

        IEnumerable<HospitalInfo> GetHospitals();
        // ✅ For editing doctor details
        ApplicationUser GetDoctorById(string id);
        bool UpdateDoctor(ApplicationUserViewModel model);

        

        PagedResult<ApplicationUserViewModel> SearchDoctor(int PageNumber, int PageSize, string Speciality = null);
        bool UpdateDoctor(ApplicationUser userModel);
    }
}
