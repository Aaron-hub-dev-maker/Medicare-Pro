using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? City { get; set; }

        public string? Description { get; set; }

        public int? HospitalId { get; set; }
        public HospitalInfo Hospital { get; set; }


        public ICollection<ApplicationUser>Employees { get; set; }
    }
}
