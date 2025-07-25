using Hospital.Models;
using Hospitals.Repositories.Interfaces;
using Hospital.ViewModels;
using Hospitals.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services
{
    public class RoomService : IRoomService
    {
        private IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void DeleteRoom(int id)
        {
            var model = _unitOfWork.GenericRepository<Room>().GetById(id);
            _unitOfWork.GenericRepository<Room>().Delete(model);
            _unitOfWork.Save();
        }

        public PagedResult<RoomViewModel> GetAll(int pageNumber, int pageSize)
        {
            int totalCount;
            List<RoomViewModel> vmList = new List<RoomViewModel>();

            try
            {
                int excludeRecords = (pageSize * pageNumber) - pageSize;

                var modelList = _unitOfWork.GenericRepository<Room>()
                    .GetAll(includeProperties: "Hospital")
                    .Skip(excludeRecords)
                    .Take(pageSize)
                    .ToList();

                totalCount = _unitOfWork.GenericRepository<Room>().GetAll().Count();

                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }

            var result = new PagedResult<RoomViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
        }

        public RoomViewModel GetRoomById(int RoomId)
        {
            var model = _unitOfWork.GenericRepository<Room>().GetById(RoomId);
            return new RoomViewModel(model);
        }

        public void InsertRoom(RoomViewModel Room)
        {
            var model = new Room
            {
                // Do NOT set Id here - let DB generate it
                RoomNumber = Room.RoomNumber,
                Type = Room.Type,
                Status = Room.Status,
                HospitalId = Room.HospitalInfoId
            };

            _unitOfWork.GenericRepository<Room>().Add(model);
            _unitOfWork.Save();
        }

        public void UpdateRoom(RoomViewModel Room)
        {
            var model = _unitOfWork.GenericRepository<Room>().GetById(Room.Id);

            if (model != null)
            {
                model.Type = Room.Type;
                model.RoomNumber = Room.RoomNumber;
                model.Status = Room.Status;
                model.HospitalId = Room.HospitalInfoId;

                _unitOfWork.GenericRepository<Room>().Update(model);
                _unitOfWork.Save();
            }
            else
            {
                throw new Exception("Room not found");
            }
        }

        public List<HospitalInfo> GetHospitals()
        {
            return _unitOfWork.GenericRepository<HospitalInfo>().GetAll().ToList();
        }

        private List<RoomViewModel> ConvertModelToViewModelList(List<Room> modelList)
        {
            return modelList.Select(x => new RoomViewModel(x)).ToList();
        }
    }
}

