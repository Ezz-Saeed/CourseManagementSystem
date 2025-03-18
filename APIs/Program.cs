
using APIs.Data;
using APIs.Extensions;
using APIs.Helpers;
using APIs.Interfaces;
using APIs.Models;
using APIs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddIdentity<Appuser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            var connection = builder.Configuration.GetConnectionString("connection") ?? 
                throw new NullReferenceException("connection couldn't be found");
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connection);
            });
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddSwagerService();
            var app = builder.Build();

            app.UseSwaggerService();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
