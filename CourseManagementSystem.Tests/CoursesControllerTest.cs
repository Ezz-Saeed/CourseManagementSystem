using APIs.Controllers;
using APIs.Data;
using APIs.DTOs.CourseDtos;
using APIs.DTOs.TrainerDtos;
using APIs.Models;
using AutoMapper;
using CourseManagementSystem.Tests.Data;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.DependencyResolver;
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
        private readonly Mock<UserManager<Appuser>> _userManager;

        public CoursesControllerTest()
        {
            _appDbContext = DbContextInMemory.GetInMemoryDbContext();
            //_userManager = UserManagerInMemory.GetInMemoryUserManager(_appDbContext);
            _userManager = new Mock<UserManager<Appuser>>(
                Mock.Of<IUserStore<Appuser>>(), null, null, null, null, null, null, null, null);
            _mapper = A.Fake<IMapper>();
            _coursesController = new CoursesController(_appDbContext, _mapper, _userManager.Object);
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


        // Delete endppoint
        [Fact]
        // Deleting existing course
        public async Task DeleteCourse_WhenCourseExists_ReturnsOk()
        {
            // Arrange
            var course = new Course
            {
                Name = "Test Course",
                Description = "Sample Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2),
                IsDeleted = false
            };

            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            // Act
            var result = await _coursesController.DeleteCourse(course.Id);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var deletedCourse = await _appDbContext.Courses.FindAsync(course.Id);
            Assert.NotNull(deletedCourse);
            // Ensure course is marked as deleted
            Assert.True(deletedCourse.IsDeleted); 
        }


        [Fact]
        // Trying to delete non existing cousrse
        public async Task DeleteCourse_WhenCourseDoesNotExist_ReturnsNotFound()
        {
            // Act
            var result = await _coursesController.DeleteCourse(999); // Non-existent ID

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        // AssignCourseToTrainer
        [Fact]
        // Suuccessfil assignment
        public async Task AssignCourseToTrainer_WhenCourseAndTrainerExist_ReturnsOk()
        {
            // Arrange
            var course = new Course
            {
                Name = "Test Course",
                Description = "Sample Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2)
            };

            var trainer = new Appuser
            {
                Id = "trainer123",
                Email = "trainer@example.com",
                Courses = new List<Course>()
            };

            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            // Mock UserManager
            var userManagerMock = new Mock<UserManager<Appuser>>(
                Mock.Of<IUserStore<Appuser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByEmailAsync(trainer.Email)).ReturnsAsync(trainer);
            userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<Appuser>())).ReturnsAsync(IdentityResult.Success);

            // Mock Mapper
            var mapperMock = new Mock<IMapper>();
            var expectedTrainerDto = new GetTrainerDto { Email = trainer.Email, Courses = new List<CourseDto>() };
            mapperMock.Setup(m => m.Map<GetTrainerDto>(It.IsAny<Appuser>())).Returns(expectedTrainerDto);

            var controller = new CoursesController(_appDbContext, mapperMock.Object, userManagerMock.Object);

            var dto = new AssignCourseToTrainerDto { CourseId = course.Id, TrainerEmail = trainer.Email };

            // Act
            var result = await controller.AssignCourseToTrainer(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var updatedTrainer = okResult.Value as GetTrainerDto;
            Assert.NotNull(updatedTrainer);
            Assert.Equal(trainer.Email, updatedTrainer.Email);
        }


        [Fact]
        // Failed assignment non-existing course entity
        public async Task AssignCourseToTrainer_WhenCourseDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AssignCourseToTrainerDto { CourseId = 999, TrainerEmail = "trainer@example.com" };

            var trainer = new Appuser { Id = "trainer123", Email = dto.TrainerEmail, Courses = new List<Course>() };
            _userManager.Setup(x => x.FindByEmailAsync(trainer.Email)).ReturnsAsync(trainer);
            _userManager.Setup(x => x.UpdateAsync(It.IsAny<Appuser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _coursesController.AssignCourseToTrainer(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }


        [Fact]
        // Trying to assign course to non-existing trainer
        public async Task AssignCourseToTrainer_WhenTrainerDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var course = new Course {
                Name = "Test Course",
                Description = "Sample Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2)
            };
            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            var dto = new AssignCourseToTrainerDto { CourseId = course.Id, TrainerEmail = "nonexistent@example.com" };

            _userManager.Setup(x => x.FindByEmailAsync(dto.TrainerEmail)).ReturnsAsync((Appuser?)null);
            _userManager.Setup(x => x.UpdateAsync(It.IsAny<Appuser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _coursesController.AssignCourseToTrainer(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }


        [Fact]
        // Update filed 
        public async Task AssignCourseToTrainer_WhenUpdateFails_ReturnsBadRequest()
        {
            // Arrange
            var course = new Course { 
                Name = "Test Course",
                Description = "Sample Description",
                StartDate = DateTime.Now.AddMonths(-2),
                EndDate = DateTime.Now.AddMonths(2)
            };
            var trainer = new Appuser { Id = "trainer123", Email = "trainer@example.com", Courses = new List<Course>() };

            await _appDbContext.Courses.AddAsync(course);
            await _appDbContext.SaveChangesAsync();

            var dto = new AssignCourseToTrainerDto { CourseId = course.Id, TrainerEmail = trainer.Email };

            var failedIdentityResult = IdentityResult.Failed(new IdentityError { Description = "Update failed" });

            var userManagerMock = new Mock<UserManager<Appuser>>(
                Mock.Of<IUserStore<Appuser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.FindByEmailAsync(dto.TrainerEmail)).ReturnsAsync(trainer);
            userManagerMock.Setup(x => x.UpdateAsync(trainer)).ReturnsAsync(failedIdentityResult);

            var controller = new CoursesController(_appDbContext, _mapper, userManagerMock.Object);

            // Act
            var result = await controller.AssignCourseToTrainer(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Update failed", badRequestResult.Value);
        }

    }
}
