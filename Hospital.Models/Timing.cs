using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Timing
    {
        public int Id { get; set; }

        public string DoctorId { get; set; }

        public ApplicationUser Doctor { get; set; }

        public DateTime Date { get; set; }

        public int MorningShiftStartTime { get; set; }

        public int MorningShiftEndTime { get; set; }

        public int AfternoonShiftStartTime { get; set; }

        public int AfternoonShiftEndTime { get; set; }

        public int Duration { get; set; }

        public Status Status { get; set; }



    }
}

namespace Hospital.Models
{
    public enum Status
    {
        [Display(Name = "On Duty")]
        OnDuty,

        [Display(Name = "Off Duty")]
        OffDuty
    }
}