using API.Controllers.Auth;
using API.Models;
using API.Models.Auth;
using API.Contracts.Auth;
using Core.Interfaces.Services;
using Core.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Fixtures;
using Xunit;

namespace PeerLearn.Tests.Unit.Otp;

public class OtpControllerTests
{
    private readonly Mock<IOtpService> _mockOtpService;
    private readonly OtpController _controller;

    public OtpControllerTests()
    {
        _mockOtpService = new Mock<IOtpService>();
        _controller = new OtpController(_mockOtpService.Object);
    }

    #region SendOtp Tests

    [Fact]
    public async Task SendOtp_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = TestData.Auth.CreateSendOtpRequest("test@example.com");
        var expirationTime = DateTime.UtcNow.AddMinutes(5);
        _mockOtpService.Setup(s => s.SendOtpAsync(request.Email)).ReturnsAsync(expirationTime);

        // Act
        var result = await _controller.SendOtp(request);

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<OtpResponse>("OTP sent successfully");
        response.Data.ExpiresAt.Should().NotBeNull();
        response.Data.ExpiresAt!.Value.Should().Be(expirationTime);
    }

    [Fact]
    public async Task SendOtp_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var request = TestData.Auth.CreateSendOtpRequest("nonexistent@example.com");
        _mockOtpService.Setup(s => s.SendOtpAsync(request.Email))
            .ThrowsAsync(AppException.CreateError("USER_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.SendOtp(request));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("USER_002");
    }

    [Fact]
    public async Task SendOtp_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.SendOtp(null!);

        // Assert
        result.ShouldBeBadRequest();
    }

    #endregion

    #region VerifyOtp Tests

    [Fact]
    public async Task VerifyOtp_ShouldReturnOk_WhenCodeIsValid()
    {
        // Arrange
        var request = TestData.Auth.CreateOtpVerifyRequest("test@example.com", "123456");
        _mockOtpService.Setup(s => s.VerifyOtpAsync(request.Email, request.Code))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.VerifyOtp(request);

        // Assert
        result.ShouldBeSuccessfulApiResponse<EmptyResponse>("OTP verified successfully");
    }

    [Fact]
    public async Task VerifyOtp_ShouldReturnNotFound_WhenOtpNotFound()
    {
        // Arrange
        var request = TestData.Auth.CreateOtpVerifyRequest("test@example.com");
        _mockOtpService.Setup(s => s.VerifyOtpAsync(request.Email, request.Code))
            .ThrowsAsync(AppException.CreateError("OTP_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.VerifyOtp(request));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("OTP_001");
    }

    [Fact]
    public async Task VerifyOtp_ShouldReturnBadRequest_WhenOtpExpired()
    {
        // Arrange
        var request = TestData.Auth.CreateOtpVerifyRequest("test@example.com");
        _mockOtpService.Setup(s => s.VerifyOtpAsync(request.Email, request.Code))
            .ThrowsAsync(AppException.CreateError("OTP_EXPIRED"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.VerifyOtp(request));

        // Assert
        ex.StatusCode.Should().Be(400);
        ex.ErrorCode.Should().Be("OTP_002");
    }

    [Fact]
    public async Task VerifyOtp_ShouldReturnBadRequest_WhenOtpIsInvalid()
    {
        // Arrange
        var request = TestData.Auth.CreateOtpVerifyRequest("test@example.com", "000000");
        _mockOtpService.Setup(s => s.VerifyOtpAsync(request.Email, request.Code))
            .ThrowsAsync(AppException.CreateError("INVALID_OTP"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.VerifyOtp(request));

        // Assert
        ex.StatusCode.Should().Be(400);
        ex.ErrorCode.Should().Be("OTP_003");
    }

    #endregion
}

