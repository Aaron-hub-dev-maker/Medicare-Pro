using Hospital.Models;
using Hospital.ViewModels;
using Hospitals.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IContactService
    {
        PagedResult<ContactViewModel> GetAll(int pageNumber, int pageSize);

        ContactViewModel GetContactById(int ContactId);

        void UpdateContact(ContactViewModel contact);

        void InsertContact(ContactViewModel contact);

        void DeleteContact(int Id);


        IEnumerable<HospitalInfo> GetHospitals();
    }
}
