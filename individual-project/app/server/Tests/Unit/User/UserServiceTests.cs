using Core.Services.User;
using Infrastructure.Repositories;
using Core.Models;
using Core.DTOs;
using Core.Exceptions;
using FluentAssertions;
using Moq;
using PeerLearn.Tests.Common.Builders;
using Xunit;
using UserModel = Core.Models.User;

namespace PeerLearn.Tests.Unit.User;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    #region GetByIdUser Tests

    [Fact]
    public async Task GetByIdUser_ShouldReturnUserDto_WhenUserExists()
    {
        // Arrange
        var user = new UserBuilder().WithId(1).Build();
        _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdUser(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetByIdUser_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _userService.GetByIdUser(999));
        
        exception.ErrorCode.Should().Be("USER_002");
    }

    #endregion

    #region SearchUsers Tests

    [Fact]
    public async Task SearchUsers_ShouldReturnUsers_WhenQueryIsValid()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new UserBuilder().WithUsername("testuser1").Build(),
            new UserBuilder().WithUsername("testuser2").Build()
        };
        _mockUserRepository.Setup(r => r.SearchAsync("test", 8)).ReturnsAsync(users);

        // Act
        var result = await _userService.SearchUsers("test", 8);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchUsers_ShouldReturnEmptyList_WhenQueryIsEmpty()
    {
        // Act
        var result = await _userService.SearchUsers("", 8);

        // Assert
        result.Should().BeEmpty();
        _mockUserRepository.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task SearchUsers_ShouldCapLimit_WhenLimitExceedsMaximum()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.SearchAsync("test", 25)).ReturnsAsync(new List<UserModel>());

        // Act
        await _userService.SearchUsers("test", 100);

        // Assert
        _mockUserRepository.Verify(r => r.SearchAsync("test", 25), Times.Once);
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser_WhenUpdatesAreValid()
    {
        // Arrange
        var user = new UserBuilder().WithId(1).Build();
        var updates = new Dictionary<string, object> { { "FirstName", "Updated" } };

        _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(user)).ReturnsAsync(user);

        // Act
        await _userService.UpdateUser(1, updates);

        // Assert
        user.FirstName.Should().Be("Updated");
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var updates = new Dictionary<string, object> { { "FirstName", "Updated" } };
        _mockUserRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _userService.UpdateUser(999, updates));
        
        exception.ErrorCode.Should().Be("USER_002");
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public async Task DeleteUser_ShouldSucceed_WhenUserExists()
    {
        // Arrange
        var user = new UserBuilder().WithId(1).Build();
        _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        await _userService.DeleteUser(1);

        // Assert
        _mockUserRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ShouldThrowException_WhenDeletionFails()
    {
        // Arrange
        var user = new UserBuilder().WithId(1).Build();
        _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(async () =>
            await _userService.DeleteUser(1));
        
        exception.ErrorCode.Should().Be("USER_008");
    }

    #endregion
}

