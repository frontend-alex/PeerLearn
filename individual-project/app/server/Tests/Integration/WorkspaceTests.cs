using API.Models.Auth;
using API.Contracts.Workspace;
using API.Models;
using Core.Enums;
using FluentAssertions;
using PeerLearn.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PeerLearn.Tests.Integration;

public class WorkspaceTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public WorkspaceTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var registerRequest = new RegisterRequest
        {
            Username = "workspaceuser",
            FirstName = "Workspace",
            LastName = "User",
            Email = "workspace@example.com",
            Password = "Password123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "workspace@example.com",
            Password = "Password123!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
    }

    [Fact]
    public async Task CreateWorkspace_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsync();

        var request = new CreateWorkspaceRequest
        {
            Name = "Test Workspace",
            Visibility = WorkspaceVisibility.Private,
            ColorHex = "#FFFFFF"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/workspace", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<WorkspaceResponse>>();
        content.Should().NotBeNull();
        content!.Data.Name.Should().Be("Test Workspace");
    }

    [Fact]
    public async Task GetWorkspace_ShouldReturnOk_WhenWorkspaceExists()
    {
        // Arrange
        await AuthenticateAsync();

        var createRequest = new CreateWorkspaceRequest
        {
            Name = "My Workspace",
            Visibility = WorkspaceVisibility.Public,
            ColorHex = "#000000"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/workspace", createRequest);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<WorkspaceResponse>>();
        var workspaceId = createContent!.Data.Id;

        // Act
        var response = await _client.GetAsync($"/api/workspace/{workspaceId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<WorkspaceResponse>>();
        content.Should().NotBeNull();
        content!.Data.Id.Should().Be(workspaceId);
    }
}
