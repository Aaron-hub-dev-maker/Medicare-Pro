using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Hospital.Models;
using Hospital.Utilities;
using Hospitals.Utilities;

namespace Hospital.web.Areas.Identity.Pages.Account
{
    public class PatientRegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<PatientRegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private IWebHostEnvironment _webHostEnvironment;

        public PatientRegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<PatientRegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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

            [Required]
            [Display(Name = "Full Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Gender")]
            public Gender Gender { get; set; }

            [Required]
            [Display(Name = "Nationality")]
            public string Nationality { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "Date of Birth")]
            public DateTime DOB { get; set; }

            [Display(Name = "Profile Picture URL")]
            public string PictureUrl { get; set; }

            [Display(Name = "Blood Group")]
            public string BloodGroup { get; set; }

            [Display(Name = "Medical History")]
            public string MedicalHistory { get; set; }

            [Required]
            [Display(Name = "I confirm that I am registering as a patient")]
            public bool IsPatient { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            System.Diagnostics.Debug.WriteLine("[PatientRegister] OnPostAsync called");
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("[PatientRegister] ModelState is valid");
                var user = CreateUser();
                user.Name = Input.Name;
                user.Address = Input.Address;
                user.City = Input.City;
                user.Nationality = Input.Nationality;
                user.DOB = Input.DOB;
                user.Gender = Input.Gender;
                user.PictureUri = string.IsNullOrEmpty(Input.PictureUrl) ? "/images/users/default-patient.png" : Input.PictureUrl;
                user.IsPatient = Input.IsPatient;
                user.BloodGroup = Input.BloodGroup;
                user.MedicalHistory = Input.MedicalHistory;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    System.Diagnostics.Debug.WriteLine("[PatientRegister] User creation succeeded");
                    _logger.LogInformation("Patient created a new account with password.");
                    await _userManager.AddToRoleAsync(user, WebSiteRoles.WebSite_Patient);
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        System.Diagnostics.Debug.WriteLine("[PatientRegister] Redirecting to RegisterConfirmation");
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[PatientRegister] Signing in user");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"[PatientRegister] Error: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[PatientRegister] ModelState is NOT valid");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PatientRegister] Validation error in {key}: {error.ErrorMessage}");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
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
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
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