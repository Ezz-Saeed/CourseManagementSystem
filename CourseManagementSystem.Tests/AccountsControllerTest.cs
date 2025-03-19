using APIs.Controllers;
using APIs.DTOs;
using APIs.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class AccountsControllerTest
{
    private readonly IAuthService _authService;
    private readonly AccountsController _controller;

    public AccountsControllerTest()
    {
        _authService = A.Fake<IAuthService>(); // Mock the service
        _controller = new AccountsController(_authService);
    }

    // Register endpoint

    [Fact]
    // Succeeded registeration
    public async Task Register_WhenModelIsValid_ReturnsOk()
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

        var authDto = new AuthDto
        {
            IsAuthenticated = true,
            Email = registerDto.Email,
            Username = registerDto.UserName,
            Roles = new List<string> { "User" },
            Token = "valid_token",
            RefreshToken = "refresh_token",
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
        };

        A.CallTo(() => _authService.RegisterAsync(A<RegisterDto>._)).Returns(authDto);

        // Mock context, response and cookies
        var responseMock = new Mock<HttpResponse>();
        var cookieCollectionMock = new Mock<IResponseCookies>();

        responseMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Response).Returns(responseMock.Object);

        _controller.ControllerContext = new ControllerContext { HttpContext = contextMock.Object };

        // Act
        var result = await _controller.Register(registerDto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var response = result.Value as AuthDto;
        Assert.True(response.IsAuthenticated);
        Assert.Equal(registerDto.Email, response.Email);
    }


    [Fact]
    // Registering existing user
    public async Task Register_WhenUserAlreadyExists_ReturnsUnauthorized()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            UserName = "existingUser",
            Email = "existing@example.com",
            Password = "Secure@123",
            FirstName = "Alice",
            LastName = "Smith",
        };

        var authDto = new AuthDto { Message = "Already registered account!" };

        A.CallTo(() => _authService.RegisterAsync(A<RegisterDto>._)).Returns(authDto);

        // Act
        var result = await _controller.Register(registerDto) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
        var response = result.Value as AuthDto;
        Assert.Equal("Already registered account!", response.Message);
        Assert.False(response.IsAuthenticated);
    }

    // GetToken endpoint

    [Fact]
    // Successfully login and get JWT token
    public async Task GetToken_WhenCredentialsAreValid_ReturnsOk()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "test@example.com", Password = "Secure@123" };

        var authDto = new AuthDto
        {
            IsAuthenticated = true,
            Email = loginDto.Email,
            Username = "testuser",
            Roles = new List<string> { "User" },
            Token = "valid_token",
            RefreshToken = "refresh_token",
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
        };

        A.CallTo(() => _authService.GetTokenAsync(A<LoginDto>._)).Returns(authDto);

        // Mock Http response with it's cookies collection
        var responseMock = new Mock<HttpResponse>();
        var cookiesMock = new Mock<IResponseCookies>();

        responseMock.Setup(r=>r.Cookies).Returns(cookiesMock.Object);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Response).Returns(responseMock.Object);

        _controller.ControllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        // Act
        var result = await _controller.GetTokenAsnc(loginDto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var response = result.Value as AuthDto;
        Assert.True(response.IsAuthenticated);
        Assert.Equal(loginDto.Email, response.Email);
    }

    [Fact]
    // Login with invalid user credentials
    public async Task GetToken_WhenCredentialsAreInvalid_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Email = "wrong@example.com", Password = "WrongPassword" };

        var authDto = new AuthDto { Message = "Invalid email or password!", IsAuthenticated = false };

        A.CallTo(()=>_authService.GetTokenAsync(A<LoginDto>._)).Returns(authDto);

        // Act
        var result = await _controller.GetTokenAsnc(loginDto) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
        var response = result.Value as AuthDto;
        Assert.False(response.IsAuthenticated);
        Assert.Equal("Invalid email or password!", response.Message);
    }

    // RefreshToken endpoint

    [Fact]
    public async Task RefreshToken_WhenValidRefreshTokenProvided_ReturnsNewTokens()
    {
        // Arrange
        var authDto = new AuthDto
        {
            IsAuthenticated = true,
            Token = "new_valid_token",
            RefreshToken = "new_refresh_token",
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
        };       

        A.CallTo(() => _authService.RefreshTokenAsync(A<string>._)).Returns(authDto);
        // Mock HttpRequest and cookies
        var refreshTokenValue = "valid_refresh_token";
        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(c => c["refreshToken"]).Returns(refreshTokenValue);
        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r=>r.Cookies).Returns(cookiesMock.Object);

        // Mock HttpResponse with it's Cookies
        var responseMock = new Mock<HttpResponse>();
        var cookieCollectionMock = new Mock<IResponseCookies>();
        responseMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c=>c.Request).Returns(requestMock.Object);
        contextMock.Setup(c=>c.Response).Returns(responseMock.Object);

        _controller.ControllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        // Act
        var result = await _controller.RefreshToken() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var response = result.Value as AuthDto;
        Assert.True(response.IsAuthenticated);
        Assert.NotNull(response.Token);
        Assert.NotNull(response.RefreshToken);
    }

    [Fact]
    // Trying to refresh JWT token when refresh token is inactive
    public async Task RefreshToken_WhenInvalidRefreshTokenProvided_ReturnsUnauthorized()
    {
        // Arrange        
        A.CallTo(() => _authService.RefreshTokenAsync(A<string>._))
            .Returns(new AuthDto { IsAuthenticated = false, Message = "Invalid refresh token!" });

        // Mock HttpRequest and cookies
        var refreshTokenValue = "valid_refresh_token";
        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(c => c["refreshToken"]).Returns(refreshTokenValue);
        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);

        // Mock HttpResponse with it's Cookies
        var responseMock = new Mock<HttpResponse>();
        var cookieCollectionMock = new Mock<IResponseCookies>();
        responseMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Request).Returns(requestMock.Object);
        contextMock.Setup(c => c.Response).Returns(responseMock.Object);

        _controller.ControllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        // Act
        var result = await _controller.RefreshToken() as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
        Assert.Equal("Invalid refresh token!", result.Value);
    }

    [Fact]
    // No refresh token provided
    public async Task RefreshToken_WhenThereIsNoRefreshTokenProvided_ReturnsUnAuthorized()
    {
        // Arrange
        // Mock HttpRequest and cookies
        var cookiesMock = new Mock<IRequestCookieCollection>();
        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Request).Returns(requestMock.Object);

        _controller.ControllerContext = new ControllerContext() { HttpContext = contextMock.Object };

        // Act
        var result = await _controller.RefreshToken() as UnauthorizedResult;

        // Assert
        Assert.Equal(401, result.StatusCode);
    }
}
