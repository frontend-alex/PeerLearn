using API.Models;
using API.Models.Auth;
using API.Contracts.Auth;
using FluentAssertions;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Common.Fixtures;
using PeerLearn.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PeerLearn.Tests.Integration.Otp;

public class OtpEndpointsTests : BaseIntegrationTest
{
    public OtpEndpointsTests(IntegrationTestFactory factory) : base(factory) { }

    #region SendOtp Tests

    [Fact]
    public async Task SendOtp_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var email = await RegisterUserAsync();
        var request = TestData.Auth.CreateSendOtpRequest(email);

        // Act
        var response = await Client.PostAsJsonAsync("/api/otp/send", request);

        // Assert
        response.ShouldBeOk();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<OtpResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task SendOtp_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var request = TestData.Auth.CreateSendOtpRequest("nonexistent@example.com");

        // Act
        var response = await Client.PostAsJsonAsync("/api/otp/send", request);

        // Assert
        response.ShouldBeNotFound();
    }

    #endregion

    #region VerifyOtp Tests

    [Fact]
    public async Task VerifyOtp_ShouldReturnNotFound_WhenOtpNotFound()
    {
        // Arrange
        var email = await RegisterUserAsync();
        var request = TestData.Auth.CreateOtpVerifyRequest(email);

        // Act
        var response = await Client.PutAsJsonAsync("/api/otp/verify", request);

        // Assert
        response.ShouldBeNotFound();
    }

    [Fact]
    public async Task VerifyOtp_ShouldReturnBadRequest_WhenCodeIsInvalid()
    {
        // Arrange
        var email = await RegisterUserAsync();
        var sendRequest = TestData.Auth.CreateSendOtpRequest(email);
        await Client.PostAsJsonAsync("/api/otp/send", sendRequest);

        var verifyRequest = TestData.Auth.CreateOtpVerifyRequest(email, "000000");

        // Act
        var response = await Client.PutAsJsonAsync("/api/otp/verify", verifyRequest);

        // Assert
        response.ShouldBeBadRequest();
    }

    #endregion
}

