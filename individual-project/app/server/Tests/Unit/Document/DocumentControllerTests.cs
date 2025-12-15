using API.Controllers.Document;
using API.Models;
using API.Contracts.Document;
using Core.Interfaces.Services;
using Core.Exceptions;
using Core.DTOs;
using Core.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Common.Fixtures;
using Xunit;

namespace PeerLearn.Tests.Unit.Document;

public class DocumentControllerTests : BaseControllerTest
{
    private readonly Mock<IDocumentService> _mockDocumentService;
    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        _mockDocumentService = new Mock<IDocumentService>();
        _controller = new DocumentController(_mockDocumentService.Object);
        _controller.ControllerContext = CreateAuthenticatedControllerContext();
    }

    #region CreateDocument Tests

    [Fact]
    public async Task CreateDocument_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = TestData.Document.CreateDocumentRequest(TestWorkspaceId);
        var documentDto = new DocumentDto { Id = TestDocumentId, Title = request.Title, WorkspaceId = TestWorkspaceId };
        _mockDocumentService.Setup(s => s.CreateDocumentAsync(TestUserId, request.WorkspaceId, request.Title, request.Kind, request.Visibility, request.ColorHex))
            .ReturnsAsync(documentDto);

        // Act
        var result = await _controller.CreateDocument(request);

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<DocumentResponse>("Document created successfully.");
        response.Data.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task CreateDocument_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var request = TestData.Document.CreateDocumentRequest(TestWorkspaceId);
        _mockDocumentService.Setup(s => s.CreateDocumentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DocumentKind>(), It.IsAny<WorkspaceVisibility?>(), It.IsAny<string?>()))
            .ThrowsAsync(AppException.CreateError("USER_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.CreateDocument(request));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("USER_002");
    }

    [Fact]
    public async Task CreateDocument_ShouldReturnForbidden_WhenAccessDenied()
    {
        // Arrange
        var request = TestData.Document.CreateDocumentRequest(TestWorkspaceId);
        _mockDocumentService.Setup(s => s.CreateDocumentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DocumentKind>(), It.IsAny<WorkspaceVisibility?>(), It.IsAny<string?>()))
            .ThrowsAsync(AppException.CreateError("WORKSPACE_ACCESS_DENIED"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.CreateDocument(request));

        // Assert
        ex.StatusCode.Should().Be(403);
        ex.ErrorCode.Should().Be("WORKSPACE_002");
    }

    #endregion

    #region GetDocument Tests

    [Fact]
    public async Task GetDocument_ShouldReturnOk_WhenDocumentExists()
    {
        // Arrange
        var documentDto = new DocumentDto { Id = TestDocumentId, Title = "Test Document", WorkspaceId = TestWorkspaceId };
        _mockDocumentService.Setup(s => s.GetDocumentAsync(TestDocumentId, TestUserId)).ReturnsAsync(documentDto);

        // Act
        var result = await _controller.GetDocument(TestDocumentId);

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<DocumentResponse>("Document retrieved successfully.");
        response.Data.Id.Should().Be(TestDocumentId);
    }

    [Fact]
    public async Task GetDocument_ShouldReturnNotFound_WhenDocumentNotFound()
    {
        // Arrange
        _mockDocumentService.Setup(s => s.GetDocumentAsync(TestDocumentId, TestUserId))
            .ThrowsAsync(AppException.CreateError("DOCUMENT_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.GetDocument(TestDocumentId));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("DOCUMENT_001");
    }

    #endregion

    #region UpdateDocument Tests

    [Fact]
    public async Task UpdateDocument_ShouldReturnOk_WhenUpdateIsValid()
    {
        // Arrange
        var request = TestData.Document.CreateUpdateDocumentRequest("Updated Title");
        var documentDto = new DocumentDto { Id = TestDocumentId, Title = request.Title! };
        _mockDocumentService.Setup(s => s.UpdateDocumentAsync(TestDocumentId, TestUserId, request.Title, request.Content, request.IsArchived, request.ColorHex, request.Visibility))
            .ReturnsAsync(documentDto);

        // Act
        var result = await _controller.UpdateDocument(TestDocumentId, request);

        // Assert
        result.ShouldBeSuccessfulApiResponse<DocumentResponse>("Document updated successfully.");
    }

    [Fact]
    public async Task UpdateDocument_ShouldReturnNotFound_WhenDocumentNotFound()
    {
        // Arrange
        var request = TestData.Document.CreateUpdateDocumentRequest("Updated Title");
        _mockDocumentService.Setup(s => s.UpdateDocumentAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<string?>(), It.IsAny<WorkspaceVisibility?>()))
            .ThrowsAsync(AppException.CreateError("DOCUMENT_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.UpdateDocument(TestDocumentId, request));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("DOCUMENT_001");
    }

    #endregion

    #region DeleteDocument Tests

    [Fact]
    public async Task DeleteDocument_ShouldReturnOk_WhenDeletionIsSuccessful()
    {
        // Arrange
        _mockDocumentService.Setup(s => s.DeleteDocumentAsync(TestDocumentId, TestUserId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteDocument(TestDocumentId);

        // Assert
        result.ShouldBeSuccessfulApiResponse<object>("Document deleted successfully.");
    }

    [Fact]
    public async Task DeleteDocument_ShouldReturnNotFound_WhenDocumentNotFound()
    {
        // Arrange
        _mockDocumentService.Setup(s => s.DeleteDocumentAsync(TestDocumentId, TestUserId))
            .ThrowsAsync(AppException.CreateError("DOCUMENT_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.DeleteDocument(TestDocumentId));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("DOCUMENT_001");
    }

    #endregion
}

