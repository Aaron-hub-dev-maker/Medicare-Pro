using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Authorization;

namespace Hospital.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing rooms in the admin area.
    /// </summary>
    [Area("Admin")]
    public class RoomsController : Controller
    {
        private readonly IRoomService _room;

        public RoomsController(IRoomService room)
        {
            _room = room;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            var customResult = _room.GetAll(pageNumber, pageSize);
            var cloudResult = new PagedResult<RoomViewModel>
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
        public IActionResult Create()
        {
            var vm = new RoomViewModel
            {
                Hospitals = _room.GetHospitals()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Create(RoomViewModel vm)
        {
            if (vm.HospitalInfoId == 0)
            {
                ModelState.AddModelError("HospitalInfoId", "Please select a valid hospital.");
                vm.Hospitals = _room.GetHospitals();
                return View(vm);
            }
            _room.InsertRoom(vm);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Edit(int id)
        {
            var vm = _room.GetRoomById(id);
            vm.Hospitals = _room.GetHospitals();
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Edit(RoomViewModel vm)
        {
            if (vm.HospitalInfoId == 0)
            {
                ModelState.AddModelError("HospitalInfoId", "Please select a valid hospital.");
                vm.Hospitals = _room.GetHospitals();
                return View(vm);
            }
            _room.UpdateRoom(vm);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Doctor")]
        public IActionResult Delete(int id)
        {
            _room.DeleteRoom(id);
            return RedirectToAction("Index");
        }
    }
}

