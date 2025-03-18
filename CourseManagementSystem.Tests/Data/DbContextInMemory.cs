using APIs.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagementSystem.Tests.Data
{
    public class DbContextInMemory
    {
        public static AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}") // Unique DB per test
                .Options;

            var context = new AppDbContext(options);

            // Ensure the database is created with the seeded data
            context.Database.EnsureCreated();

            return context;
        }
    }
}
