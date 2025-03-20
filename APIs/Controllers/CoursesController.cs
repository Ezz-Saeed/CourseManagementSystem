using APIs.Data;
using APIs.DTOs.CourseDtos;
using APIs.DTOs.TrainerDtos;
using APIs.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(AppDbContext context, IMapper mapper, UserManager<Appuser> userManager) : ControllerBase
    {

        [HttpGet("GetCourses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await context.Courses.Where(c=>!c.IsDeleted).ToListAsync();
            var coursesToReturn = mapper.Map<List<GetCourseDto>>(courses);
            return Ok(coursesToReturn);
        }

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

        [HttpPut("assignCourseToTrainer")]
        public async Task<IActionResult> AssignCourseToTrainer(AssignCourseToTrainerDto dto)
        {
            var course = await context.Courses.FindAsync(dto.CourseId);
            if (course is null) return BadRequest();
            var trainer = await userManager.FindByEmailAsync(dto.TrainerEmail);
            if (trainer is null) return BadRequest();

            trainer.Courses!.Add(course);
            var result = await userManager.UpdateAsync(trainer);

            if (!result.Succeeded) return BadRequest($"{string.Join(", ", result.Errors.Select(e=>e.Description))}");

            var updatedTrainer = mapper.Map<GetTrainerDto>(trainer);
            return Ok(updatedTrainer);
        }
    }
}
