using API.Models.Auth;
using API.Contracts.Document;
using API.Contracts.Workspace;
using API.Models;
using Core.Enums;
using FluentAssertions;
using PeerLearn.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PeerLearn.Tests.Integration;

public class DocumentTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public DocumentTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var registerRequest = new RegisterRequest
        {
            Username = "docuser",
            FirstName = "Doc",
            LastName = "User",
            Email = "doc@example.com",
            Password = "Password123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "doc@example.com",
            Password = "Password123!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
    }

    private async Task<int> CreateWorkspaceAsync()
    {
        var request = new CreateWorkspaceRequest
        {
            Name = "Doc Workspace",
            Visibility = WorkspaceVisibility.Private,
            ColorHex = "#FFFFFF"
        };
        var response = await _client.PostAsJsonAsync("/api/workspace", request);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<WorkspaceResponse>>();
        return content!.Data.Id;
    }

    [Fact]
    public async Task CreateDocument_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        await AuthenticateAsync();
        var workspaceId = await CreateWorkspaceAsync();

        var request = new CreateDocumentRequest
        {
            WorkspaceId = workspaceId,
            Title = "Test Document",
            Kind = DocumentKind.Document, 
            Visibility = WorkspaceVisibility.Private,
            ColorHex = "#FFFFFF"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/document", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<DocumentResponse>>();
        content.Should().NotBeNull();
        content!.Data.Title.Should().Be("Test Document");
    }

    [Fact]
    public async Task GetDocument_ShouldReturnOk_WhenDocumentExists()
    {
        // Arrange
        await AuthenticateAsync();
        var workspaceId = await CreateWorkspaceAsync();

        var createRequest = new CreateDocumentRequest
        {
            WorkspaceId = workspaceId,
            Title = "My Document",
            Kind = DocumentKind.Document,
            Visibility = WorkspaceVisibility.Private,
            ColorHex = "#FFFFFF"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/document", createRequest);
        var createContent = await createResponse.Content.ReadFromJsonAsync<ApiResponse<DocumentResponse>>();
        var documentId = createContent!.Data.Id;

        // Act
        var response = await _client.GetAsync($"/api/document/{documentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<DocumentResponse>>();
        content.Should().NotBeNull();
        content!.Data.Id.Should().Be(documentId);
    }
}
