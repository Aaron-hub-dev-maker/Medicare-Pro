using Hospital.Models;
using Hospital.ViewModels;
using Hospitals.Repositories.Interfaces;
using Hospitals.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public class HospitalInfoService : IHospitalInfo
    {
        private IUnitOfWork _unitOfWork;

        public HospitalInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void DeleteHospitalInfo(int id)
        {
            var model = _unitOfWork.GenericRepository<HospitalInfo>().GetById(id);
            _unitOfWork.GenericRepository<HospitalInfo>().Delete(model);
            _unitOfWork.Save();
        }

        public PagedResult<HospitalInfoViewModel> GetAll(int pageNumber, int pageSize)
        {
            int totalCount;
            List<HospitalInfoViewModel> vmList = new List<HospitalInfoViewModel>();

            try
            {
                int ExcludeRecords = (pageSize * pageNumber) - pageSize;

                // ✅ Apply alphabetical ordering before pagination
                var modelList = _unitOfWork.GenericRepository<HospitalInfo>()
                    .Table
                    .OrderBy(h => h.Name)
                    .Skip(ExcludeRecords)
                    .Take(pageSize)
                    .ToList();

                totalCount = _unitOfWork.GenericRepository<HospitalInfo>()
                    .GetAll()
                    .Count();

                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }

            return new PagedResult<HospitalInfoViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public HospitalInfoViewModel GetHospitalById(int HospitalId)
        {
            var model = _unitOfWork.GenericRepository<HospitalInfo>().GetById(HospitalId);
            var vm = new HospitalInfoViewModel(model);
            return vm;
        }

        public void InsertHospitalInfo(HospitalInfoViewModel hospitalInfo)
        {
            var model = new HospitalInfoViewModel().ConvertViewModel(hospitalInfo);
            _unitOfWork.GenericRepository<HospitalInfo>().Add(model);
            _unitOfWork.Save();
        }

        public void InsertHospitalInfo(HospitalInfo hospitalModel)
        {
            _unitOfWork.GenericRepository<HospitalInfo>().Add(hospitalModel);
            _unitOfWork.Save();
        }

        public void UpdateHospitalInfo(HospitalInfoViewModel hospitalInfo)
        {
            var ModelById = _unitOfWork.GenericRepository<HospitalInfo>().GetById(hospitalInfo.Id);

            if (ModelById == null)
            {
                throw new Exception($"Hospital with ID {hospitalInfo.Id} not found."); //
            }

            ModelById.Name = hospitalInfo.Name;
            ModelById.Type = hospitalInfo.Type;
            ModelById.City = hospitalInfo.City;
            ModelById.PinCode = hospitalInfo.PinCode;
            ModelById.Country = hospitalInfo.Country;
            //ModelById.Description = hospitalInfo.Description;

            _unitOfWork.GenericRepository<HospitalInfo>().Update(ModelById);
            _unitOfWork.Save();
        }

        public void UpdateHospitalInfo(HospitalInfo hospitalModel)
        {
            var existing = _unitOfWork.GenericRepository<HospitalInfo>().GetById(hospitalModel.Id);
            if (existing != null)
            {
                existing.Name = hospitalModel.Name;
                existing.Type = hospitalModel.Type;
                existing.City = hospitalModel.City;
                existing.PinCode = hospitalModel.PinCode;
                existing.Country = hospitalModel.Country;
                existing.Description = hospitalModel.Description;

                _unitOfWork.GenericRepository<HospitalInfo>().Update(existing);
                _unitOfWork.Save();
            }
        }

        private List<HospitalInfoViewModel> ConvertModelToViewModelList(List<HospitalInfo> modelList)
        {
            return modelList.Select(x => new HospitalInfoViewModel(x)).ToList();
        }
    }
}
