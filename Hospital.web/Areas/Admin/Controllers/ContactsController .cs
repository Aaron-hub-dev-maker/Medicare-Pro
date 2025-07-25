using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing contacts in the admin area.
    /// </summary>
    [Area("Admin")]
    public class ContactsController : Controller
    {
        private readonly IContactService _contact;

        public ContactsController(IContactService contact)
        {
            _contact = contact;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            var customResult = _contact.GetAll(pageNumber, pageSize);
            var cloudResult = new PagedResult<ContactViewModel>
            {
                Data = customResult.Data,
                PageNumber = customResult.PageNumber,
                PageSize = customResult.PageSize,
                TotalItems = customResult.TotalItems
            };
            return View(cloudResult);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Edit(int id)
        {
            var viewModel = _contact.GetContactById(id);
            viewModel.Hospitals = _contact.GetHospitals();
            ViewBag.Hospitals = viewModel.Hospitals;
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Edit(ContactViewModel vm)
        {
            if (vm.HospitalInfoId == 0)
            {
                ModelState.AddModelError("HospitalInfoId", "Please select a valid hospital.");
                vm.Hospitals = _contact.GetHospitals();
                ViewBag.Hospitals = vm.Hospitals;
                return View(vm);
            }
            _contact.UpdateContact(vm);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Create()
        {
            var vm = new ContactViewModel
            {
                Hospitals = _contact.GetHospitals()
            };
            ViewBag.Hospitals = vm.Hospitals;
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Create(ContactViewModel vm)
        {
            if (vm.HospitalInfoId == 0)
            {
                ModelState.AddModelError("HospitalInfoId", "Please select a valid hospital.");
                vm.Hospitals = _contact.GetHospitals();
                ViewBag.Hospitals = vm.Hospitals;
                return View(vm);
            }
            _contact.InsertContact(vm);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Delete(int id)
        {
            _contact.DeleteContact(id);
            return RedirectToAction("Index");
        }
    }
}
