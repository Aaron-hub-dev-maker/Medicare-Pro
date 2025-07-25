using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace Hospital.ViewModels
{
    public class ApplicationUserViewModel
    {
        public List<ApplicationUser> Doctors { get; set; } = new List<ApplicationUser>();

        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string City { get; set; }

        public Gender Gender { get; set; }

        public bool IsDoctor { get; set; }

        public string Specialist { get; set; }

        public string PictureUri { get; set; }

        //public int? DepartmentId { get; set; }

        //public string DepartmentName { get; set; }

        public string HospitalName { get; set; }

        public string Department { get; set; }

        [Required(ErrorMessage = "Please select a hospital.")]
        public int? HospitalInfoId { get; set; }

        //public string? HospitalInfoId { get; set; }




       // public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> Hospitals { get; set; }

        public ApplicationUserViewModel()
        {
            Doctors = new List<ApplicationUser>();
            Hospitals = new List<SelectListItem>();
        }

        public ApplicationUserViewModel(ApplicationUserViewModel doctor)
        {
            
        }
        public ApplicationUserViewModel(ApplicationUser user, IEnumerable<HospitalInfo> hospitalList)
        {
            Id = user.Id;
            Name = user.Name;
            City = user.City;
            Gender = user.Gender;
            IsDoctor = user.IsDoctor;
            Specialist = user.Specialist;
            UserName = user.UserName;
            Email = user.Email;
            HospitalInfoId = user.HospitalInfoId;
            PictureUri = user.PictureUri;
            HospitalName = hospitalList?.FirstOrDefault(h => h.Id == user.HospitalInfoId)?.Name; // ✅ Keep existing variable
            Hospitals = hospitalList.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = h.Name
            }).ToList();

            // Department = user.Department?.Name;
            //HospitalInfoId = user.Department?.Hospital?.Name;

            // DepartmentId = user.Department?.Id;
            //DepartmentName = user.Department?.Name ?? "";
            //HospitalName = user.Department?.Hospital?.Name ?? "";

        }

        public ApplicationUser ConvertViewModelToModel()
        {
            return new ApplicationUser { 
                
                Id = this.Id,
                Name = this.Name,
                City = this.City,
                Gender = this.Gender,
                IsDoctor = this.IsDoctor,
                Specialist = this.Specialist,
                Email = this.Email,
                UserName = this.UserName,
                HospitalInfoId = this.HospitalInfoId
                //DepartmentId = user.DepartmentId

            };
        }
    }
}
