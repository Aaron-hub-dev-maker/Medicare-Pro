using Hospital.ViewModels;
using Hospitals.Utilities;
using Hospital.Models;  // For HospitalInfo
using System.Collections.Generic;

namespace Hospital.Services
{
    public interface IRoomService
    {
        PagedResult<RoomViewModel> GetAll(int pageNumber, int pageSize);

        RoomViewModel GetRoomById(int RoomId);

        void UpdateRoom(RoomViewModel Room);

        void InsertRoom(RoomViewModel Room);

        void DeleteRoom(int RoomId);

        List<HospitalInfo> GetHospitals();   // <-- New method added here
    }
}
