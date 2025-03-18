using APIs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace APIs.Data
{
    public class AppDbContext : IdentityDbContext<Appuser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public virtual DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Roles seed
            var adminRoleId = "role-admin";
            builder.Entity<IdentityRole>().HasData
                (
                    new IdentityRole() { Id = adminRoleId, Name = "Admin", 
                        NormalizedName = "Admin".ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString()},

                    new IdentityRole()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Trainer",
                        NormalizedName = "Trainer".ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    }
            );

            // Course seed
            builder.Entity<Course>().HasData(
            new Course
            {
                Id = 1,
                Name = "ASP.NET Core Web API",
                Description = "Learn to build RESTful APIs using ASP.NET Core.",
                StartDate = new DateTime(2025, 4, 1),
                EndDate = new DateTime(2025, 6, 1),
            },
            new Course
            {
                Id = 2,
                Name = "Angular Frontend Development",
                Description = "A comprehensive course on Angular for building SPAs.",
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 7, 1),
            },

            new Course
            {
                Id = 3,
                Name = "Database Management System",
                Description = "A comprehensive course on SQL SERVER.",
                StartDate = new DateTime(2025, 5, 1),
                EndDate = new DateTime(2025, 7, 1),
            }
        );

            // Seed for admin user
            var adminId = "admin-123";
            var admin = new Appuser
            {
                Id = adminId,
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                FirstName = "Admin",
                LastName = "User",
            };

            // Hash password
            var hasher = new PasswordHasher<Appuser>();
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
            builder.Entity<Appuser>().HasData(admin);

            // Assign the admin user to the admin role
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminId,
                    RoleId = adminRoleId
                }
            );

            base.OnModelCreating(builder);
        }
    }
}
