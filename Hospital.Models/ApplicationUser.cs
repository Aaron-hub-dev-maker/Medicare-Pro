
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public string Name { get; set; }

        public Gender Gender { get; set; }

        public string? Nationality { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Description { get; set; }

        public DateTime DOB { get; set; }

        public string? Specialist { get; set; }

        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }

        public string? PictureUri { get; set; }
        public int? DepartmentId { get; set; }  // nullable FK
        public string? BloodGroup { get; set; }
        public string? MedicalHistory { get; set; }
       
        //public Department Department { get; set; }
        
        public ICollection<Appointment>Appointments { get; set; }
        [NotMapped]
        public ICollection<Payroll> Payrolls { get; set; }

        public string? ProfilePicturePath { get; set; }

        public int? HospitalInfoId { get; set; }
        //public string? HospitalInfoId { get; set; }
        public HospitalInfo? HospitalInfo { get; set; }

        public ICollection<Timing> Timings { get; set; }


    }
}

namespace Hospital.Models
{
    public enum Gender
    {
        Male,Female,Other
    }
}