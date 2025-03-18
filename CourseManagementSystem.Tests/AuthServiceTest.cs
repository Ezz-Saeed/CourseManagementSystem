using APIs.Data;
using APIs.DTOs;
using APIs.Helpers;
using APIs.Models;
using APIs.Services;
using CourseManagementSystem.Tests.Data;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CourseManagementSystem.Tests
{
    public class AuthServiceTest
    {
        private readonly UserManager<Appuser> _userManager;
        private readonly IOptions<JWT> _jwtOptions;
        private readonly AuthService _sut;
        private readonly AppDbContext _appDbContext;

        public AuthServiceTest()
        {
            // Fake dependencies
            _appDbContext = DbContextInMemory.GetInMemoryDbContext();
            _userManager = UserManagerInMemory.GetInMemoryUserManager(_appDbContext);

            var jwtSettings = new JWT
            {
                Key = "ThisIsASecretKeyForTestingPurposes123456",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                DurationInMinutes = 60
            };
            _jwtOptions = Options.Create(jwtSettings);


            _sut = new AuthService(_userManager, _jwtOptions);
        }

        
        [Fact]
        // Test for register existing account
        public async Task RegisterAsync_WhenEmailOrUserNameExists_ReturnsUnAuthenticatedResult()
        {
            var existingUser = new Appuser
            {
                UserName = "existingUser",
                Email = "existing@example.com",
                FirstName = "John",
                LastName = "Doe",
            };
            await _userManager.CreateAsync(existingUser, "Test@123");

            var registerDto = new RegisterDto
            {
                UserName = "existingUser",
                Email = "existing@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "Test@123"
            };

            // Act
            var result = await _sut.RegisterAsync(registerDto);

            // Assert: Ensure method returns error message
            Assert.False(result.IsAuthenticated);
            Assert.Null(result.Email);
            Assert.Equal("Already registered account!", result.Message);

        }

        
        [Fact]
        // Test register new user
        public async Task RegisterAsync_WhenNewUserRegistered_ReturnsAuthenticatedResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "newUser",
                Email = "newuser@example.com",
                FirstName = "Alice",
                LastName = "Smith",
                Password = "Secure@123"
            };

            // Act
            var result = await _sut.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.IsAuthenticated);
            Assert.Equal(registerDto.Email, result.Email);
            Assert.Contains("Trainer", result.Roles);
            Assert.NotNull(result.Token);
        }


        
        [Fact]
        // Register with weak password
        public async Task RegisterAsync_WhenWeakPassword_ReturnsErrorMessage()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "newUser",
                Email = "newuser@example.com",
                FirstName = "Alice",
                LastName = "Smith",
                Password = "123"  // Weak password
            };

            // Act
            var result = await _sut.RegisterAsync(registerDto);

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.Contains("Passwords must be at least", result.Message);
        }

    }
}