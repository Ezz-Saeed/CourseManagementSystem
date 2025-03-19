using APIs.Data;
using APIs.DTOs.CourseDtos;
using APIs.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        [HttpPost("addCourse")]
        public async Task<IActionResult> AddCourse(AddCourseDto dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var course = mapper.Map<Course>(dto);
            await context.AddAsync(course);
            var result = await context.SaveChangesAsync();
            if(result <= 0) return BadRequest("Couldn't add course");
            var createdCourse = mapper.Map<GetCourseDto>(course);
            return Ok(createdCourse);
        }
    }
}
