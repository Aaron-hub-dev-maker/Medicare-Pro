using Hospital.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public int HospitalInfoId { get; set; }

        public HospitalInfo HospitalInfo { get; set; }

        public string HospitalName { get; set; }

        public List<HospitalInfo> Hospitals { get; set; } = new List<HospitalInfo>();

        // Default constructor
        public RoomViewModel() { }

        // Constructor that initializes from a Room model
        public RoomViewModel(Room model)
        {
            Id = model.Id;
            RoomNumber = model.RoomNumber;
            Type = model.Type;
            Status = model.Status;
            HospitalInfoId = model.HospitalId;
            HospitalInfo = model.Hospital;
        }

        // Convert this ViewModel instance to a Room model
        public Room ConvertViewModel()
        {
            return new Room
            {
                Id = this.Id,
                Type = this.Type,
                RoomNumber = this.RoomNumber,
                Status = this.Status,
                HospitalId = this.HospitalInfoId,
                Hospital = this.HospitalInfo
            };
        }
    }
}
