using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Hospital.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing users (doctors, patients) in the admin area.
    /// </summary>
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly IApplicationUserService _userService;

        public UsersController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            return View(_userService.GetAll(pageNumber, pageSize));
        }

        public IActionResult AllDoctors(int pageNumber = 1, int pageSize = 10)
        {
            var pagedDoctors = _userService.GetAllDoctor(pageNumber, pageSize);
            return View(pagedDoctors);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            var doctor = _userService.GetDoctorById(id);
            if (doctor == null)
                return NotFound();
            var hospitals = _userService.GetHospitals();
            var doctorViewModel = new ApplicationUserViewModel(doctor, hospitals);
            return View(doctorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var hospitals = _userService.GetHospitals();
                model.Hospitals = hospitals.Select(h => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                });
                return View(model);
            }
            var userModel = model.ConvertViewModelToModel();
            bool result = _userService.UpdateDoctor(userModel);
            if (!result)
                return NotFound();
            return RedirectToAction(nameof(AllDoctors));
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            
            var result = _userService.Delete(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to delete doctor. They may have associated appointments, timings, or patient reports.";
                return RedirectToAction(nameof(AllDoctors));
            }
            
            TempData["SuccessMessage"] = "Doctor deleted successfully!";
            return RedirectToAction(nameof(AllDoctors));
        }
    }
}
