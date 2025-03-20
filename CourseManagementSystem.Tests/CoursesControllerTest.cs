using APIs.Controllers;
using APIs.Data;
using APIs.DTOs.CourseDtos;
using APIs.Models;
using AutoMapper;
using CourseManagementSystem.Tests.Data;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagementSystem.Tests
{
    public class CoursesControllerTest
    {
        private readonly AppDbContext _appDbContext;
        private readonly CoursesController _coursesController;
        private readonly IMapper _mapper;

        public CoursesControllerTest()
        {
            _appDbContext = DbContextInMemory.GetInMemoryDbContext();
            _mapper = A.Fake<IMapper>();
            _coursesController = new CoursesController(_appDbContext, _mapper);
        }

        // Add Course

        [Fact]
        // Succefully Adding course
        public async Task AddCourse_WhenCourseIsAddedSuccessfully_ReturnsOk()
        {
            // Arrange         
            var dto = new AddCourseDto
            {
                Name = "Test Course",
                Description = "A sample course",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(4),
            };

            var course = new Course { Name = dto.Name, Description = dto.Description };
            var getCourseDto = new GetCourseDto
            {
                Id = course.Id,
                Name = course.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            A.CallTo(() => _mapper.Map<Course>(dto)).Returns(course);
            A.CallTo(() => _mapper.Map<GetCourseDto>(course)).Returns(getCourseDto);

            // Act
            var result = await _coursesController.AddCourse(dto) as OkObjectResult;

            // Assert
            Assert.Equal(200, result!.StatusCode);
            var response = result.Value as GetCourseDto;
            Assert.Equal(getCourseDto.Name, response!.Name);
        }

        //[Fact]

        //public async Task AddCourse_WhenSaveFails_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var dto = new AddCourseDto { Name = "Test Course", Description = "A sample course", 
        //        StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(4),
        //    };
        //    var course = new Course { Name = dto.Name, Description = dto.Description, 
        //        StartDate = dto.StartDate, EndDate = dto.EndDate};

        //    A.CallTo(() => _mapper.Map<Course>(dto)).Returns(course);

        //    // Simulate failed save (do not save changes)
        //    //_appDbContext.ChangeTracker.Clear();
        //    _appDbContext.ChangeTracker.

        //    // Act
        //    var result = await _coursesController.AddCourse(dto) as BadRequestObjectResult;

        //    // Assert
        //    Assert.Equal(400, result!.StatusCode);
        //    Assert.Equal("Couldn't add course", result.Value);
        //}

        [Fact]
        // Validation errors when trying to add new course
        public async Task AddCourse_WhenModelStateIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            _coursesController.ModelState.AddModelError("Name", "Required");

            var dto = new AddCourseDto
            {
                Description = "Missing name field",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(4),
            };

            // Act
            var result = await _coursesController.AddCourse(dto) as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, result!.StatusCode);
        }


        // Update course
        [Fact]
        // Successfull update
        public async Task UpdateCourse_WhenCourseExists_ReturnsOk()
        {
            // Arrange
            var course = new Course
            {
                Name = "Old Course",
                Description = "Old Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2)
            };

            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            var dto = new UpdateCourseDto
            {
                Name = "Updated Course",
                Description = "Updated Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3)
            };

            var expectedDto = new GetCourseDto { Name = dto.Name, Description = dto.Description,
                StartDate = (DateTime) dto.StartDate, EndDate = (DateTime) dto.EndDate};
            A.CallTo(() => _mapper.Map<GetCourseDto>(course)).Returns(expectedDto);

            // Act
            var result = await _coursesController.UpdateCourse(course.Id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var updatedCourse = okResult.Value as GetCourseDto;
            Assert.NotNull(updatedCourse);
            Assert.Equal(dto.Name, updatedCourse.Name);
            Assert.Equal(dto.Description, updatedCourse.Description);
        }

        [Fact]
        // Trying to update non existing entity
        public async Task UpdateCourse_WhenCourseDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateCourseDto
            {
                Name = "Updated Course",
                Description = "Updated Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3)
            };

            // Act
            var result = await _coursesController.UpdateCourse(999, dto); // Non-existent ID

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact]
        // No changes
        public async Task UpdateCourse_WhenDtoHasNoChanges_ReturnsSameCourse()
        {
            // Arrange
            var course = new Course
            {
                Name = "Same Course",
                Description = "Same Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2)
            };

            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            var dto = new UpdateCourseDto(); // Empty DTO means no changes

            var expectedDto = new GetCourseDto
            {
                Name = course.Name,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate
            };

            A.CallTo(() => _mapper.Map<GetCourseDto>(course)).Returns(expectedDto);

            // Act
            var result = await _coursesController.UpdateCourse(course.Id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var updatedCourse = okResult.Value as GetCourseDto;
            Assert.NotNull(updatedCourse);
            Assert.Equal(course.Name, updatedCourse.Name);
            Assert.Equal(course.Description, updatedCourse.Description);
        }


        [Fact]
        // Partially Update course entity
        public async Task UpdateCourse_WhenPartialUpdateSent_UpdatesOnlySpecifiedFields()
        {
            // Arrange
            var course = new Course
            {
                Name = "Initial Name",
                Description = "Initial Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2)
            };

            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            var dto = new UpdateCourseDto
            {
                Name = "Updated Name" // Only updating Name field
            };

            var expectedDto = new GetCourseDto
            {
                Name = dto.Name,
                Description = course.Description, // Should remain unchanged
                StartDate = course.StartDate,
                EndDate = course.EndDate
            };

            A.CallTo(() => _mapper.Map<GetCourseDto>(course)).Returns(expectedDto);

            // Act
            var result = await _coursesController.UpdateCourse(course.Id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var updatedCourse = okResult.Value as GetCourseDto;
            Assert.NotNull(updatedCourse);
            Assert.Equal(dto.Name, updatedCourse.Name);
            // Remains unchanged
            Assert.Equal(course.Description, updatedCourse.Description); 
        }

    }
}
