using System;
using System.Collections.Generic;

namespace Hospital.Models
{
    public class PatientReport
    {
        public int Id { get; set; }

        public string Diagnose { get; set; }

        // Foreign Key for Doctor
        public string DoctorId { get; set; }
        public ApplicationUser Doctor { get; set; }

        // Foreign Key for Patient
        public string PatientId { get; set; }
        public ApplicationUser Patient { get; set; }

        // Related prescribed medicines
        public ICollection<PrescribedMedicine> PrescribedMedicine { get; set; }
    }
}
