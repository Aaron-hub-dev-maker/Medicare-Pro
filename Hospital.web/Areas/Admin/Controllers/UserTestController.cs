using Hospital.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hospital.Web.Controllers
{
    public class UserTestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserTestController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> TestCreateUser()
        {
            Console.WriteLine(">>> TestCreateUser Method HIT <<<");

            if (_userManager == null)
            {
                Console.WriteLine(">>> UserManager IS NULL <<<");
                return Content("Error: _userManager is not initialized!");
            }

            var testUser = new ApplicationUser
            {
                UserName = "testdoctor@example.com",
                Email = "testdoctor@example.com",
                IsDoctor = true,
                Name = "Test Doctor",
                City = "Test City",
                Specialist = "Test Specialist",
                DepartmentId = 1
            };

            Console.WriteLine(">>> Attempting User Creation <<<");

            var result = await _userManager.CreateAsync(testUser, "Test@12345");

            Console.WriteLine(">>> UserManager.CreateAsync Executed <<<");

            if (!result.Succeeded)
            {
                Console.WriteLine(">>> User Creation FAILED <<<");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
                return Content("User creation failed. Check console logs for details.");
            }

            Console.WriteLine(">>> User Registered Successfully <<<");
            return Content("User registration successful!");
        }

    }
}
