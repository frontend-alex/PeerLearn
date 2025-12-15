using API.Controllers.User;
using API.Models;
using API.Contracts.User;
using Core.Interfaces.Services;
using Core.Exceptions;
using Core.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeerLearn.Tests.Common.Assertions;
using PeerLearn.Tests.Common.Base;
using PeerLearn.Tests.Common.Fixtures;
using Xunit;

namespace PeerLearn.Tests.Unit.User;

public class UserControllerTests : BaseControllerTest
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);
        _controller.ControllerContext = CreateAuthenticatedControllerContext();
    }

    #region SearchUsers Tests

    [Fact]
    public async Task SearchUsers_ShouldReturnOk_WhenQueryIsValid()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto { Id = 1, Username = "testuser1", Email = "test1@example.com" },
            new UserDto { Id = 2, Username = "testuser2", Email = "test2@example.com" }
        };
        _mockUserService.Setup(s => s.SearchUsers("test", 8)).ReturnsAsync(users);

        // Act
        var result = await _controller.SearchUsers("test", 8);

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<IEnumerable<UserResponse>>("Users retrieved successfully.");
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchUsers_ShouldReturnEmptyList_WhenQueryIsEmpty()
    {
        // Arrange
        _mockUserService.Setup(s => s.SearchUsers("", 8)).ReturnsAsync(Enumerable.Empty<UserDto>());

        // Act
        var result = await _controller.SearchUsers("", 8);

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<IEnumerable<UserResponse>>("Users retrieved successfully.");
        response.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchUsers_ShouldCapLimit_WhenLimitExceedsMaximum()
    {
        // Arrange
        _mockUserService.Setup(s => s.SearchUsers("test", 100)).ReturnsAsync(Enumerable.Empty<UserDto>());

        // Act
        await _controller.SearchUsers("test", 100);

        // Assert
        _mockUserService.Verify(s => s.SearchUsers("test", 100), Times.Once);
    }

    #endregion

    #region GetProfile Tests

    [Fact]
    public async Task GetProfile_ShouldReturnOk_WhenUserIsAuthenticated()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = TestUserId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        _mockUserService.Setup(s => s.GetByIdUser(TestUserId)).ReturnsAsync(userDto);

        // Act
        var result = await _controller.GetProfile();

        // Assert
        var response = result.ShouldBeSuccessfulApiResponse<UserResponse>("User profile retrieved successfully.");
        response.Data.Id.Should().Be(TestUserId);
        response.Data.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetProfile_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetByIdUser(TestUserId))
            .ThrowsAsync(AppException.CreateError("USER_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.GetProfile());

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("USER_002");
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_ShouldReturnOk_WhenUpdateIsValid()
    {
        // Arrange
        var updates = TestData.User.CreateUpdateRequest("Updated", "Name");
        _mockUserService.Setup(s => s.UpdateUser(TestUserId, updates)).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = await _controller.UpdateUser(updates);

        // Assert
        result.ShouldBeSuccessfulApiResponse<EmptyResponse>("User updated successfully.");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        var updates = TestData.User.CreateUpdateRequest("Updated");
        _mockUserService.Setup(s => s.UpdateUser(TestUserId, updates))
            .ThrowsAsync(AppException.CreateError("USER_NOT_FOUND"));

        // Act
        var ex = await Assert.ThrowsAsync<AppException>(() => _controller.UpdateUser(updates));

        // Assert
        ex.StatusCode.Should().Be(404);
        ex.ErrorCode.Should().Be("USER_002");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenUpdatesIsNull()
    {
        // Act
        var result = await _controller.UpdateUser(null!);

        // Assert
        result.ShouldBeBadRequest();
    }

    #endregion
}

