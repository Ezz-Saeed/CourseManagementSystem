using APIs.Data;
using APIs.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagementSystem.Tests.Data
{
    public class UserManagerInMemory
    {
        public static UserManager<Appuser> GetInMemoryUserManager(AppDbContext context)
        {
            // Create UserStore with In-Memory Context
            var userStore = new UserStore<Appuser>(context);

            // Identity options
            var options = Options.Create(new IdentityOptions());

            // Required services
            var passwordHasher = new PasswordHasher<Appuser>();
            var userValidators = new List<IUserValidator<Appuser>> { new UserValidator<Appuser>() };
            var passwordValidators = new List<IPasswordValidator<Appuser>> { new PasswordValidator<Appuser>() };
            var lookupNormalizer = new UpperInvariantLookupNormalizer();
            var identityErrorDescriber = new IdentityErrorDescriber();
            var logger = new Logger<UserManager<Appuser>>(new LoggerFactory());

            // Return in-memory UserManager
            return new UserManager<Appuser>(
                userStore,
                options,
                passwordHasher,
                userValidators,
                passwordValidators,
                lookupNormalizer,
                identityErrorDescriber,
                null,  // No services needed
                logger);
        }
    }
}
