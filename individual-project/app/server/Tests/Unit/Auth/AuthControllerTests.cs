using API.Controllers.Auth;
using API.Models;
using API.Models.Auth;
using API.Contracts.Auth;
using Core.Interfaces.Services;
using Core.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Common.Fixtures;
using Xunit;

namespace PeerLearn.Tests.Unit.Auth;

public class AuthControllerTests : BaseControllerTest
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;
    private readonly Mock<IResponseCookies> _mockCookies;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
        
        var (httpContext, mockCookies) = CreateHttpContextWithCookies();
        _mockCookies = mockCookies;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    #region Register Tests

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = TestData.Auth.CreateRegisterRequest();
        _mockAuthService.Setup(s => s.RegisterAsync(request.Username, request.FirstName, request.LastName, request.Email, request.Password))
            .ReturnsAsync(request.Email);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<RegisterResponse>("User has successfully registered.");
        response.Data.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailExists()
    {
        // Arrange
        var request = TestData.Auth.CreateRegisterRequest();
        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(AppException.CreateError("EMAIL_EXISTS"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.Register(request));

        // Assert
        ex.StatusCode.Should().Be(409);
        ex.ErrorCode.Should().Be("USER_001");
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenUsernameExists()
    {
        // Arrange
        var request = TestData.Auth.CreateRegisterRequest();
        _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(AppException.CreateError("USERNAME_EXISTS"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.Register(request));

        // Assert
        ex.StatusCode.Should().Be(409);
        ex.ErrorCode.Should().Be("USER_003");
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.Register(null!);

        // Assert
        result.ShouldBeBadRequest();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var request = TestData.Auth.CreateLoginRequest("test@example.com");
        var token = "generated_jwt_token";
        _mockAuthService.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ReturnsAsync(token);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.ShouldBeSuccessfulApiResponse<EmptyResponse>("User has successfully logged in.");
        _mockCookies.Verify(c => c.Append("access_token", token, It.IsAny<CookieOptions>()), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = TestData.Auth.CreateLoginRequest("test@example.com");
        _mockAuthService.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ThrowsAsync(AppException.CreateError("INVALID_CREDENTIALS"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.Login(request));

        // Assert
        ex.StatusCode.Should().Be(401);
        ex.ErrorCode.Should().Be("AUTH_003");
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenEmailNotVerified()
    {
        // Arrange
        var request = TestData.Auth.CreateLoginRequest("unverified@example.com");
        _mockAuthService.Setup(s => s.LoginAsync(request.Email, request.Password))
            .ThrowsAsync(AppException.CreateError("EMAIL_NOT_VERIFIED"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.Login(request));

        // Assert
        ex.StatusCode.Should().Be(400);
        ex.ErrorCode.Should().Be("AUTH_004");
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.Login(null!);

        // Assert
        result.ShouldBeBadRequest();
    }

    #endregion

    #region Logout Tests

    [Fact]
    public void Logout_ShouldReturnOk_WhenUserIsAuthorized()
    {
        // Arrange
        var (httpContext, mockCookies) = CreateHttpContextWithCookies();
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = _controller.Logout();

        // Assert
        result.ShouldBeSuccessfulApiResponse<EmptyResponse>("Logged out successfully.");
        mockCookies.Verify(c => c.Delete("access_token"), Times.Once);
    }

    #endregion
}

