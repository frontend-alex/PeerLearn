using API.Contracts.User;
using API.Models;
using FluentAssertions;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PeerLearn.Tests.Integration.User;

public class UserEndpointsTests : BaseIntegrationTest
{
    public UserEndpointsTests(IntegrationTestFactory factory) : base(factory) { }

    #region SearchUsers Tests

    [Fact]
    public async Task SearchUsers_ShouldReturnOk_WhenQueryIsValid()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/user/search?query=test&limit=8");

        // Assert
        response.ShouldBeOk();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserResponse>>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task SearchUsers_ShouldReturnEmptyList_WhenQueryIsEmpty()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await Client.GetAsync("/api/user/search?query=&limit=8");

        // Assert
        response.ShouldBeOk();
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserResponse>>>();
        content.Should().NotBeNull();
        content!.Data.Should().BeEmpty();
    }

    #endregion

    #region GetProfile Tests

    [Fact]
    public async Task GetProfile_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await Client.GetAsync("/api/user/me");

        // Assert
        response.ShouldBeUnauthorized();
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_ShouldReturnUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var updates = new Dictionary<string, object> { { "FirstName", "Updated" } };

        // Act
        var response = await Client.PutAsJsonAsync("/api/user/update", updates);

        // Assert
        response.ShouldBeUnauthorized();
    }

    #endregion
}

