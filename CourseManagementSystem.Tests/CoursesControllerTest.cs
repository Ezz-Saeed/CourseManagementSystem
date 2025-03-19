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
    }
}
