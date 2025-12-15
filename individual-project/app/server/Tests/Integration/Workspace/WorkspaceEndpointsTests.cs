using API.Contracts.Workspace;
using API.Models;
using API.Models.Auth;
using Core.Enums;
using FluentAssertions;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PeerLearn.Tests.Integration.Workspace;

public class WorkspaceEndpointsTests : BaseIntegrationTest
{
    public WorkspaceEndpointsTests(IntegrationTestFactory factory) : base(factory) { }

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
        var response = await Client.PostAsJsonAsync("/api/workspace", request);

        // Assert
        response.ShouldBeOk();
        var content = await ReadJsonAsync<ApiResponse<WorkspaceResponse>>(response);
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
        var createResponse = await Client.PostAsJsonAsync("/api/workspace", createRequest);
        createResponse.ShouldBeOk();
        var createContent = await ReadJsonAsync<ApiResponse<WorkspaceResponse>>(createResponse);
        var workspaceId = createContent!.Data.Id;

        // Act
        var response = await Client.GetAsync($"/api/workspace/{workspaceId}");

        // Assert
        response.ShouldBeOk();
        var content = await ReadJsonAsync<ApiResponse<WorkspaceResponse>>(response);
        content.Should().NotBeNull();
        content!.Data.Id.Should().Be(workspaceId);
    }
}

