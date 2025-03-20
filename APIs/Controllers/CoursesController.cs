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

        [HttpPut("updateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseDto dto)
        {
            var course = await context.Courses.FindAsync(id);
            if(course is null) return NotFound();

            if(!string.IsNullOrEmpty(dto.Name))
                course.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description))
                course.Description = dto.Description;
            if (dto.StartDate is not null)
                course.StartDate = (DateTime) dto.StartDate;
            if (dto.EndDate is not null)
                course.EndDate = (DateTime) dto.EndDate;

            context.Update(course);
            var result = await context.SaveChangesAsync();
            if (result <= 0) return BadRequest("Couldn't update course");
            var updatedCourse = mapper.Map<GetCourseDto>(course);
            return Ok(updatedCourse);
        }

        [HttpPut("deleteCpurse/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await context.Courses.FindAsync(id);
            if(course is null) return NotFound();

            course.IsDeleted = true;
            context.Courses.Update(course);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
