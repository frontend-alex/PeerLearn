using Core.Services.Auth;
using Core.Interfaces.Services;
using Infrastructure.Repositories;
using Core.Models;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using PeerLearn.Tests.Common.Builders;
using Xunit;
using UserModel = Core.Models.User;

namespace PeerLearn.Tests.Unit.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockJwtService = new Mock<IJwtService>();
        _authService = new AuthService(_mockUserRepository.Object, _mockPasswordService.Object, _mockJwtService.Object);
    }

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_ShouldReturnEmail_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var userBuilder = new UserBuilder();
        var user = userBuilder.Build();
        var hashedPassword = "hashed_password";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((UserModel?)null);
        _mockUserRepository.Setup(r => r.ExistsByUsernameAsync(user.Username)).ReturnsAsync(false);
        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>())).Returns(hashedPassword);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<UserModel>())).ReturnsAsync(It.IsAny<UserModel>());

        // Act
        var result = await _authService.RegisterAsync(user.Username, user.FirstName, user.LastName, user.Email, "Password123!");

        // Assert
        result.Should().Be(user.Email);
        _mockUserRepository.Verify(r => r.CreateAsync(It.Is<UserModel>(u => 
            u.Username == user.Username && u.Email == user.Email && !u.EmailVerified)), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenEmailExists()
    {
        // Arrange
        var existingUser = new UserBuilder().WithEmailVerified(true).Build();
        _mockUserRepository.Setup(r => r.GetByEmailAsync(existingUser.Email)).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<AppException>(async () =>
            await _authService.RegisterAsync("newuser", "Test", "User", existingUser.Email, "Password123!"));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenUsernameExists()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((UserModel?)null);
        _mockUserRepository.Setup(r => r.ExistsByUsernameAsync(user.Username)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _authService.RegisterAsync(user.Username, user.FirstName, user.LastName, user.Email, "Password123!"));
        
        exception.ErrorCode.Should().Be("USER_003");
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new UserBuilder().WithEmailVerified(true).Build();
        var token = "jwt_token";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mockPasswordService.Setup(p => p.VerifyPassword(It.IsAny<string>(), user.PasswordHash)).Returns(true);
        _mockJwtService.Setup(j => j.GenerateToken(user)).Returns(token);

        // Act
        var result = await _authService.LoginAsync(user.Email, "Password123!");

        // Assert
        result.Should().Be(token);
        _mockJwtService.Verify(j => j.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _authService.LoginAsync("nonexistent@example.com", "Password123!"));
        
        exception.ErrorCode.Should().Be("AUTH_003");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new UserBuilder().WithEmailVerified(true).Build();
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mockPasswordService.Setup(p => p.VerifyPassword(It.IsAny<string>(), user.PasswordHash)).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _authService.LoginAsync(user.Email, "WrongPassword"));
        
        exception.ErrorCode.Should().Be("AUTH_003");
    }

    #endregion
}

