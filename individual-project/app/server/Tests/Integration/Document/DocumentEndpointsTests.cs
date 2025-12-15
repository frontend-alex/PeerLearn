using API.Contracts.Document;
using API.Contracts.Workspace;
using API.Models;
using API.Models.Auth;
using Core.Enums;
using FluentAssertions;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Common.Fixtures;
using PeerLearn.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PeerLearn.Tests.Integration.Document;

public class DocumentEndpointsTests : BaseIntegrationTest
{
    public DocumentEndpointsTests(IntegrationTestFactory factory) : base(factory) { }

    private async Task<int> CreateWorkspaceAsync()
    {
        var request = new CreateWorkspaceRequest
        {
            Name = "Doc Workspace",
            Visibility = WorkspaceVisibility.Private,
            ColorHex = "#FFFFFF"
        };
        var response = await Client.PostAsJsonAsync("/api/workspace", request);
        response.ShouldBeOk();
        var content = await ReadJsonAsync<ApiResponse<WorkspaceResponse>>(response);
        return content!.Data.Id;
    }

    [Fact]
    public async Task CreateDocument_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsync();
        var workspaceId = await CreateWorkspaceAsync();
        var request = TestData.Document.CreateDocumentRequest(workspaceId);

        // Act
        var response = await Client.PostAsJsonAsync("/api/document", request);

        // Assert
        response.ShouldBeOk();
        var content = await ReadJsonAsync<ApiResponse<DocumentResponse>>(response);
        content.Should().NotBeNull();
        content!.Data.Title.Should().Be("Test Document");
    }

    [Fact]
    public async Task GetDocument_ShouldReturnOk_WhenDocumentExists()
    {
        // Arrange
        await AuthenticateAsync();
        var workspaceId = await CreateWorkspaceAsync();
        var createRequest = TestData.Document.CreateDocumentRequest(workspaceId);
        var createResponse = await Client.PostAsJsonAsync("/api/document", createRequest);
        createResponse.ShouldBeOk();
        var createContent = await ReadJsonAsync<ApiResponse<DocumentResponse>>(createResponse);
        var documentId = createContent!.Data.Id;

        // Act
        var response = await Client.GetAsync($"/api/document/{documentId}");

        // Assert
        response.ShouldBeOk();
        var content = await ReadJsonAsync<ApiResponse<DocumentResponse>>(response);
        content.Should().NotBeNull();
        content!.Data.Id.Should().Be(documentId);
    }
}

