using Hospital.Models;
using Hospitals.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Hospitals.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Migration failed: " + ex.Message, ex);
            }

            // Seed Roles
            if (!_roleManager.RoleExistsAsync(WebSiteRoles.WebSite_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Doctor)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Patient)).GetAwaiter().GetResult();

                // Seed Default Admin User
                var adminUser = new ApplicationUser
                {
                    UserName = "Harkesh",
                    Email = "harkesh@xyz.com",
                    EmailConfirmed = true,
                    Name = "Admin User",
                    Gender = Gender.Male, // or Gender.Female/Other as appropriate
                    DOB = new DateTime(1990, 1, 1), // or any valid date
                    IsDoctor = false,
                    IsPatient = false
                };

                var result = _userManager.CreateAsync(adminUser, "Harkesh@123").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(adminUser, WebSiteRoles.WebSite_Admin).GetAwaiter().GetResult();
                }
            }

            // Seed Departments if none exist
            if (!_context.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { Name = "Cardiology" },
                    new Department { Name = "Neurology" },
                    new Department { Name = "Pediatrics" },
                    new Department { Name = "Oncology" },
                    new Department { Name = "Orthopedics" }
                };

                _context.Departments.AddRange(departments);
                _context.SaveChanges();
            }
        }
    }
}

