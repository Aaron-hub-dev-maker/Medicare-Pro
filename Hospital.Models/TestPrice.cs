using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class TestPrice
    {
        public int Id { get; set; }

        public string TestCode { get; set; }

        public decimal Price { get; set; }

        // Foreign key properties
        public int LabId { get; set; }
        public Lab Lab { get; set; }

        public int BillId { get; set; }
        public Bill Bill { get; set; }
    }
}

