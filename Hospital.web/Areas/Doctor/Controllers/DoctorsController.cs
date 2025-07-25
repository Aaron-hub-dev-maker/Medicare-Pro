using Hospital.Models;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Hospitals.Repositories.Interfaces; // Required for IUnitOfWork

namespace Hospital.web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorsController(IDoctorService doctorService, IUnitOfWork unitOfWork)
        {
            _doctorService = doctorService;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            //return View(_doctorService.GetAll(pageNumber, pageSize));
            return RedirectToAction("AddTiming");
        }

        [HttpGet]
        public IActionResult AddTiming()
        {
            Timing timing = new Timing();
            List<SelectListItem> morningShiftStart = new List<SelectListItem>();
            List<SelectListItem> morningShiftEnd = new List<SelectListItem>();
            List<SelectListItem> afternoonShiftStart = new List<SelectListItem>();
            List<SelectListItem> afternoonShiftEnd = new List<SelectListItem>();

            for (int i = 1; i <= 11; i++)
                morningShiftStart.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            for (int i = 1; i <= 13; i++)
                morningShiftEnd.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            for (int i = 13; i <= 16; i++)
                afternoonShiftStart.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            for (int i = 13; i <= 18; i++)
                afternoonShiftEnd.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            ViewBag.morningStart = new SelectList(morningShiftStart, "Value", "Text");
            ViewBag.morningEnd = new SelectList(morningShiftEnd, "Value", "Text");
            ViewBag.evenStart = new SelectList(afternoonShiftStart, "Value", "Text");
            ViewBag.evenEnd = new SelectList(afternoonShiftEnd, "Value", "Text");

            TimingViewModel vm = new TimingViewModel
            {
                ScheduleDate = DateTime.Now.AddDays(1)
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddTiming(TimingViewModel vm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claims != null)
            {
                vm.DoctorId = claims.Value;
                //vm.DoctorId = Guid.Parse(claims.Value);
                //vm.DoctorId = claims.Value;
                _doctorService.AddTiming(vm);
                _unitOfWork.Save();

                TempData["SuccessMessage"] = "Timing added successfully!";

            }


            return RedirectToAction("Index");
            //return RedirectToAction("AddTimming");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var viewModel = _doctorService.GetTimingById(id);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(TimingViewModel vm)
        {
            _doctorService.UpdateTiming(vm);
            _unitOfWork.Save(); // Save changes
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _doctorService.DeleteTiming(id); // Updated method name
            _unitOfWork.Save();              // Save deletion
            return RedirectToAction("Index");
        }
    }
}
