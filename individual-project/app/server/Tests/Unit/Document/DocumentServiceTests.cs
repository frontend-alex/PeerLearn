using Core.Services.Document;
using Core.Interfaces.repository.Document;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Workspace;
using Core.Models;
using Core.DTOs;
using Core.Enums;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using PeerLearn.Tests.Common.Builders;
using Xunit;
using UserModel = Core.Models.User;
using DocumentModel = Core.Models.Document;

namespace PeerLearn.Tests.Unit.Document;

public class DocumentServiceTests
{
    private readonly Mock<IDocumentRepository> _mockDocumentRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserWorkspaceRepository> _mockUserWorkspaceRepository;
    private readonly DocumentService _documentService;
    private const int TestUserId = 1;
    private const int TestWorkspaceId = 1;
    private const int TestDocumentId = 1;

    public DocumentServiceTests()
    {
        _mockDocumentRepository = new Mock<IDocumentRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserWorkspaceRepository = new Mock<IUserWorkspaceRepository>();
        _documentService = new DocumentService(
            _mockDocumentRepository.Object,
            _mockUserRepository.Object,
            _mockUserWorkspaceRepository.Object);
    }

    #region CreateDocumentAsync Tests

    [Fact]
    public async Task CreateDocumentAsync_ShouldReturnDocumentDto_WhenCreationIsSuccessful()
    {
        // Arrange
        var user = new UserBuilder().WithId(TestUserId).Build();
        var createdDocument = new DocumentBuilder().WithId(TestDocumentId).WithWorkspaceId(TestWorkspaceId).Build();

        _mockUserRepository.Setup(r => r.GetByIdAsync(TestUserId)).ReturnsAsync(user);
        _mockUserWorkspaceRepository.Setup(r => r.UserHasAccessAsync(TestUserId, TestWorkspaceId)).ReturnsAsync(true);
        _mockDocumentRepository.Setup(r => r.CreateAsync(It.IsAny<DocumentModel>())).ReturnsAsync(createdDocument);
        _mockDocumentRepository.Setup(r => r.GetByIdAsync(TestDocumentId)).ReturnsAsync(createdDocument);

        // Act
        var result = await _documentService.CreateDocumentAsync(TestUserId, TestWorkspaceId, "Test Document", DocumentKind.Document);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Document");
    }

    [Fact]
    public async Task CreateDocumentAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(TestUserId)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _documentService.CreateDocumentAsync(TestUserId, TestWorkspaceId, "Test Document", DocumentKind.Document));
        
        exception.ErrorCode.Should().Be("USER_002");
    }

    [Fact]
    public async Task CreateDocumentAsync_ShouldThrowException_WhenAccessDenied()
    {
        // Arrange
        var user = new UserBuilder().WithId(TestUserId).Build();
        _mockUserRepository.Setup(r => r.GetByIdAsync(TestUserId)).ReturnsAsync(user);
        _mockUserWorkspaceRepository.Setup(r => r.UserHasAccessAsync(TestUserId, TestWorkspaceId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _documentService.CreateDocumentAsync(TestUserId, TestWorkspaceId, "Test Document", DocumentKind.Document));
        
        exception.ErrorCode.Should().Be("WORKSPACE_002");
    }

    #endregion

    #region GetDocumentAsync Tests

    [Fact]
    public async Task GetDocumentAsync_ShouldReturnDocumentDto_WhenDocumentExists()
    {
        // Arrange
        var document = new DocumentBuilder().WithId(TestDocumentId).WithWorkspaceId(TestWorkspaceId).Build();
        _mockDocumentRepository.Setup(r => r.GetByIdAsync(TestDocumentId)).ReturnsAsync(document);
        _mockUserWorkspaceRepository.Setup(r => r.UserHasAccessAsync(TestUserId, TestWorkspaceId)).ReturnsAsync(true);

        // Act
        var result = await _documentService.GetDocumentAsync(TestDocumentId, TestUserId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(TestDocumentId);
    }

    [Fact]
    public async Task GetDocumentAsync_ShouldThrowException_WhenDocumentNotFound()
    {
        // Arrange
        _mockDocumentRepository.Setup(r => r.GetByIdAsync(TestDocumentId)).ReturnsAsync((DocumentModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _documentService.GetDocumentAsync(TestDocumentId, TestUserId));
        
        exception.ErrorCode.Should().Be("DOCUMENT_001");
    }

    #endregion

    #region DeleteDocumentAsync Tests

    [Fact]
    public async Task DeleteDocumentAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
    {
        // Arrange
        var document = new DocumentBuilder().WithId(TestDocumentId).WithWorkspaceId(TestWorkspaceId).Build();
        _mockDocumentRepository.Setup(r => r.GetByIdAsync(TestDocumentId)).ReturnsAsync(document);
        _mockUserWorkspaceRepository.Setup(r => r.UserIsOwnerAsync(TestUserId, TestWorkspaceId)).ReturnsAsync(true);
        _mockDocumentRepository.Setup(r => r.DeleteAsync(TestDocumentId)).ReturnsAsync(true);

        // Act
        var result = await _documentService.DeleteDocumentAsync(TestDocumentId, TestUserId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDocumentAsync_ShouldThrowException_WhenInsufficientPermissions()
    {
        // Arrange
        var document = new DocumentBuilder().WithId(TestDocumentId).WithWorkspaceId(TestWorkspaceId).Build();
        _mockDocumentRepository.Setup(r => r.GetByIdAsync(TestDocumentId)).ReturnsAsync(document);
        _mockUserWorkspaceRepository.Setup(r => r.UserIsOwnerAsync(TestUserId, TestWorkspaceId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _documentService.DeleteDocumentAsync(TestDocumentId, TestUserId));
        
        exception.ErrorCode.Should().Be("USER_WORKSPACE_003");
    }

    #endregion
}

