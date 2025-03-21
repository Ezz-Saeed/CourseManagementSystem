//using APIs.DTOs.TrainerDtos;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Reporting.NETCore;

//namespace APIs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReportsController(IWebHostEnvironment webHostEnvironment) : ControllerBase
//    {
//        public ActionResult CourseTrainerReport()
//        {
//            var reportPath = $"{webHostEnvironment.WebRootPath}/Reports/CourseTrainer.rdlc";
//            var report = new LocalReport() { ReportPath = reportPath };

//            var trainers = await userManager.Users.Where(u => !u.IsDeleted).ToListAsync();
//            var returnedTrainers = mapper.Map<List<GetTrainerDto>>(trainers);
//        }
//    }
//}
