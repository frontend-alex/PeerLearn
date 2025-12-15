using Core.Services.Auth;
using Infrastructure.Repositories;
using Core.Models;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using PeerLearn.Tests.Common.Builders;
using Xunit;
using UserModel = Core.Models.User;
using OtpModel = Core.Models.Otp;

namespace PeerLearn.Tests.Unit.Otp;

public class OtpServiceTests
{
    private readonly Mock<IOtpRepository> _mockOtpRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly OtpService _otpService;

    public OtpServiceTests()
    {
        _mockOtpRepository = new Mock<IOtpRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _otpService = new OtpService(_mockOtpRepository.Object, _mockUserRepository.Object);
    }

    #region SendOtpAsync Tests

    [Fact]
    public async Task SendOtpAsync_ShouldReturnExpirationTime_WhenUserExists()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mockOtpRepository.Setup(r => r.DeleteOtpByEmailAsync(user.Email)).Returns(Task.CompletedTask);
        _mockOtpRepository.Setup(r => r.CreateOtpAsync(It.IsAny<OtpModel>())).Returns(Task.CompletedTask);

        // Act
        var result = await _otpService.SendOtpAsync(user.Email);

        // Assert
        result.Should().BeAfter(DateTime.UtcNow);
        result.Should().BeBefore(DateTime.UtcNow.AddMinutes(6));
        _mockOtpRepository.Verify(r => r.DeleteOtpByEmailAsync(user.Email), Times.Once);
        _mockOtpRepository.Verify(r => r.CreateOtpAsync(It.Is<OtpModel>(o => o.Email == user.Email)), Times.Once);
    }

    [Fact]
    public async Task SendOtpAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _otpService.SendOtpAsync("nonexistent@example.com"));
        
        exception.ErrorCode.Should().Be("USER_002");
    }

    #endregion

    #region VerifyOtpAsync Tests

    [Fact]
    public async Task VerifyOtpAsync_ShouldSucceed_WhenOtpIsValid()
    {
        // Arrange
        var user = new UserBuilder().WithEmailVerified(false).Build();
        var otp = new OtpBuilder().WithEmail(user.Email).WithCode("123456").Build();

        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mockOtpRepository.Setup(r => r.GetOtpByEmailAsync(user.Email)).ReturnsAsync(otp);
        _mockOtpRepository.Setup(r => r.DeleteOtpByEmailAsync(user.Email)).Returns(Task.CompletedTask);
        _mockUserRepository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(user);

        // Act
        await _otpService.VerifyOtpAsync(user.Email, "123456");

        // Assert
        user.EmailVerified.Should().BeTrue();
        _mockOtpRepository.Verify(r => r.DeleteOtpByEmailAsync(user.Email), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task VerifyOtpAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _otpService.VerifyOtpAsync("nonexistent@example.com", "123456"));
        
        exception.ErrorCode.Should().Be("USER_002");
    }

    [Fact]
    public async Task VerifyOtpAsync_ShouldThrowException_WhenOtpExpired()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var otp = new OtpBuilder().WithEmail(user.Email).WithExpiredTime().Build();

        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mockOtpRepository.Setup(r => r.GetOtpByEmailAsync(user.Email)).ReturnsAsync(otp);
        _mockOtpRepository.Setup(r => r.DeleteOtpByEmailAsync(user.Email)).Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _otpService.VerifyOtpAsync(user.Email, "123456"));
        
        exception.ErrorCode.Should().Be("OTP_002");
    }

    [Fact]
    public async Task VerifyOtpAsync_ShouldThrowException_WhenOtpCodeIsInvalid()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var otp = new OtpBuilder().WithEmail(user.Email).WithCode("123456").Build();

        _mockUserRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mockOtpRepository.Setup(r => r.GetOtpByEmailAsync(user.Email)).ReturnsAsync(otp);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _otpService.VerifyOtpAsync(user.Email, "000000"));
        
        exception.ErrorCode.Should().Be("OTP_003");
    }

    #endregion
}

