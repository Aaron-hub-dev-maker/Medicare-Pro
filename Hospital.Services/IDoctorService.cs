using Hospital.ViewModels;
using Hospitals.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IDoctorService
    {
        PagedResult<TimingViewModel> GetAll(int pageNumber, int pageSize);

        IEnumerable<TimingViewModel> GetAll();

        TimingViewModel GetTimingById(int TimingId);

        void UpdateTiming(TimingViewModel timing);

        void AddTiming(TimingViewModel timing);

        void DeleteTiming(int TimingId);
    }
}
