using System;

namespace Hospital.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Type { get; set; }

        public string City { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Description { get; set; }

        // Foreign Key for Doctor
        public string DoctorId { get; set; }
        public ApplicationUser Doctor { get; set; }

        // Foreign Key for Patient
        public string PatientId { get; set; }
        public ApplicationUser Patient { get; set; }
    }
}
