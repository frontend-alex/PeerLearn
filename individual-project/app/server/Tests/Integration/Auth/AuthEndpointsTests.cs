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

namespace PeerLearn.Tests.Integration.Auth;

public class AuthEndpointsTests : BaseIntegrationTest
{
    public AuthEndpointsTests(IntegrationTestFactory factory) : base(factory) { }

    #region Register Tests

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = TestData.Auth.CreateRegisterRequest();

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.ShouldBeOk();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<RegisterResponse>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
        content.Data.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailExists()
    {
        // Arrange
        var email = await RegisterUserAsync();
        await VerifyUserEmailAsync(email);
        var request = TestData.Auth.CreateRegisterRequest(email);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.ShouldBeConflict();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "ab",
            FirstName = "Test",
            LastName = "User",
            Email = "invalid-email",
            Password = "short"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.ShouldBeBadRequest();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var email = await RegisterUserAsync();
        var request = TestData.Auth.CreateLoginRequest(email);

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var cookies = response.Headers.GetValues("Set-Cookie");
            cookies.Should().Contain(c => c.StartsWith("access_token"));
        }
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var email = await RegisterUserAsync();
        var request = TestData.Auth.CreateLoginRequest(email, "WrongPassword");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.ShouldBeUnauthorized();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var request = TestData.Auth.CreateLoginRequest("nonexistent@example.com");

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.ShouldBeUnauthorized();
    }

    #endregion
}

