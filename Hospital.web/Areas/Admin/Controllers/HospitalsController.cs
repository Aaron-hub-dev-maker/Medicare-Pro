using cloudscribe.Pagination.Models;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing hospitals in the admin area.
    /// </summary>
    [Area("Admin")]
    public class HospitalsController : Controller
    {
        private IHospitalInfo _hospitalInfo;

        public HospitalsController(IHospitalInfo hospitalInfo)
        {
            _hospitalInfo = hospitalInfo;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            return View(_hospitalInfo.GetAll(pageNumber, pageSize));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Edit(int id)
        {
            var viewModel = _hospitalInfo.GetHospitalById(id);
            return View(viewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Edit(HospitalInfoViewModel vm)
        {
            _hospitalInfo.UpdateHospitalInfo(vm);
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Create(HospitalInfoViewModel vm)
        {
            _hospitalInfo.InsertHospitalInfo(vm);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Delete(int id)
        {
            _hospitalInfo.DeleteHospitalInfo(id);
            return RedirectToAction("Index");
        }
    }
}