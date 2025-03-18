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

        // Test for GetToken method

        [Fact]
        //Login with valid credentials
        public async Task GetTokenAsync_WhenCredentialsAreValid_ReturnsAuthenticatedResult()
        {
            // Arrange
            var user = new Appuser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                RefreshTokens = new List<RefreshToken>()
            };

            await _userManager.CreateAsync(user, "Secure@123");
            await _userManager.AddToRoleAsync(user, "Trainer");

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Secure@123"
            };

            // Act
            var result = await _sut.GetTokenAsync(loginDto);

            // Assert
            Assert.True(result.IsAuthenticated);
            Assert.Equal(user.Email, result.Email);
            Assert.Contains("Trainer", result.Roles);
            Assert.NotNull(result.Token);
            Assert.True(result.ExpiresOn > DateTime.UtcNow);
        }



        [Fact]
        //Login with invalid email
        public async Task GetTokenAsync_WhenEmailIsInvalid_ReturnsErrorMessage()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "wrong@example.com",
                Password = "Secure@123"
            };

            // Act
            var result = await _sut.GetTokenAsync(loginDto);

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.Equal("Invalid email or password!", result.Message);
        }


        [Fact]
        //Login with valid password
        public async Task GetTokenAsync_WhenPasswordIsInvalid_ReturnsErrorMessage()
        {
            // Arrange
            var user = new Appuser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                RefreshTokens = new List<RefreshToken>()
            };

            await _userManager.CreateAsync(user, "Secure@123");
            await _userManager.AddToRoleAsync(user, "Trainer");

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword!"
            };

            // Act
            var result = await _sut.GetTokenAsync(loginDto);

            // Assert
            Assert.False(result.IsAuthenticated);
            Assert.Equal("Invalid email or password!", result.Message);
        }

        [Fact]
        //Login with valid credentials and user has active refresh token
        public async Task GetTokenAsync_WhenUserHasActiveRefreshToken_ReturnsSameRefreshToken()
        {
            // Arrange
            var user = new Appuser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                RefreshTokens = new List<RefreshToken>
            {
                new RefreshToken { Token = "existing_refresh_token", ExpiresOn = DateTime.UtcNow.AddDays(7), RevokedOn = null }
            }
            };

            await _userManager.CreateAsync(user, "Secure@123");
            await _userManager.AddToRoleAsync(user, "Trainer");

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Secure@123"
            };

            // Act
            var result = await _sut.GetTokenAsync(loginDto);

            // Assert
            Assert.Equal("existing_refresh_token", result.RefreshToken);
            Assert.True(result.RefreshTokenExpiration > DateTime.UtcNow);
        }

        [Fact]
        //Login with valid credentials and user has no active refresh tokens
        public async Task GetTokenAsync_WhenUserHasNoActiveRefreshToken_GeneratesNewRefreshToken()
        {
            // Arrange
            var user = new Appuser
            {
                UserName = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                RefreshTokens = new List<RefreshToken>() // No active refresh token
            };

            await _userManager.CreateAsync(user, "Secure@123");
            await _userManager.AddToRoleAsync(user, "Trainer");

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Secure@123"
            };

            // Act
            var result = await _sut.GetTokenAsync(loginDto);

            // Assert
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.RefreshTokenExpiration > DateTime.UtcNow);
        }
    }
}