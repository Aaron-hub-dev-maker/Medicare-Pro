using Hospital.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hospitals.Repositories
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<HospitalInfo> HospitalInfos { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineReport> MedicineReports { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PrescribedMedicine> PrescribedMedicines { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PatientReport> PatientReports { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<TestPrice> TestPrices { get; set; }

        public DbSet<Timing> Timings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.HospitalInfo)
            .WithMany() // or .WithMany(h => h.Users) if you define that
            .HasForeignKey(u => u.HospitalInfoId)
            .OnDelete(DeleteBehavior.SetNull); // or .Restrict, based on your preference


            /* modelBuilder.Entity<ApplicationUser>()
                 .HasOne(u => u.Department)
                 .WithMany()
                 .HasForeignKey(u => u.DepartmentId)
                 .OnDelete(DeleteBehavior.SetNull); // Or Restrict, depending on your need
            */
            // Appointment FK constraints
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey("DoctorId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey("PatientId")
                .OnDelete(DeleteBehavior.Cascade);

            // PatientReport FK constraints
            modelBuilder.Entity<PatientReport>()
                .HasOne(pr => pr.Doctor)
                .WithMany()
                .HasForeignKey("DoctorId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientReport>()
                .HasOne(pr => pr.Patient)
                .WithMany()
                .HasForeignKey("PatientId")
                .OnDelete(DeleteBehavior.Cascade);

            // TestPrice FK constraints to avoid multiple cascade path issues
            modelBuilder.Entity<TestPrice>()
                .HasOne(tp => tp.Lab)
                .WithMany() // Add collection if you want: .WithMany(l => l.TestPrices)
                .HasForeignKey(tp => tp.LabId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete on Lab

            modelBuilder.Entity<TestPrice>()
                .HasOne(tp => tp.Bill)
                .WithMany() // Add collection if you want: .WithMany(b => b.TestPrices)
                .HasForeignKey(tp => tp.BillId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete on Bill

            modelBuilder.Entity<Timing>()
                .HasOne(t => t.Doctor)
                .WithMany()   // or .WithMany(d => d.Timings) if you have a collection navigation on ApplicationUser
                .HasForeignKey(t => t.DoctorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}


