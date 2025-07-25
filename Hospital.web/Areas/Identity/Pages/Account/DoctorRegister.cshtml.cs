using Hospital.Models;
using Hospital.Utilities;
using Hospitals.Repositories;
using Hospitals.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hospital.web.Areas.Identity.Pages.Account
{
    public class DoctorRegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<DoctorRegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public DoctorRegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DoctorRegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment env,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _env = env;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string? HospitalInfoId { get; set; }

        public List<HospitalInfo> Hospitals { get; set; }


        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Name { get; set; }
            public Gender Gender { get; set; }
            public string Nationality { get; set; }
            public string City { get; set; }
            public string Address { get; set; }
            public DateTime DOB { get; set; }
            public string Specialist { get; set; }
            public bool IsDoctor { get; set; }
            public IFormFile PictureUrl { get; set; }

            [Required(ErrorMessage = "Please select a hospital.")]
            public string HospitalInfoId { get; set; }



            //public int? DepartmentId { get; set; }
            // public IEnumerable<SelectListItem> DepartmentList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Hospitals = _context.HospitalInfos.ToList();
            /*
                        Input = new InputModel
                        {
                            DepartmentList = _context.Departments.Select(d => new SelectListItem
                            {
                                Value = d.Id.ToString(),
                                Text = d.Name
                            }).ToList()
                        };*/
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {


            returnUrl ??= Url.Content("~/Admin/Users/AllDoctors");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Input.HospitalInfoId) || !int.TryParse(Input.HospitalInfoId, out var hospitalId))
                {
                    ModelState.AddModelError("Input.HospitalInfoId", "Please select a valid hospital.");
                    Hospitals = _context.HospitalInfos.ToList();
                    return Page();
                }
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.Name = Input.Name;
                user.Address = Input.Address;
                user.City = Input.City;
                user.Nationality = Input.Nationality;
                user.DOB = Input.DOB;
                user.Gender = Input.Gender;
                user.IsDoctor = Input.IsDoctor;
                user.Specialist = Input.Specialist;
                user.HospitalInfoId = hospitalId;


                //user.DepartmentId = Input.DepartmentId;

                ImageOperations image = new ImageOperations(_env);
                string filename = image.ImageUpload(Input.PictureUrl);
                user.PictureUri = filename ?? "/images/default-user.png";
                user.ProfilePicturePath = user.PictureUri;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, WebSiteRoles.WebSite_Doctor);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            /*
            Input.DepartmentList = _context.Departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();  */
            Hospitals = _context.HospitalInfos.ToList();

            return Page();
        }




        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not abstract and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}

