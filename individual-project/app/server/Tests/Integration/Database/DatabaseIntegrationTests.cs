using Infrastructure.Persistence.SQL;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeerLearn.Tests.Helpers;
using FluentAssertions;
using PeerLearn.Tests.Common.Builders;
using Xunit;

namespace PeerLearn.Tests.Integration.Database;

public class DatabaseIntegrationTests : IClassFixture<IntegrationTestFactory>, IDisposable
{
    private readonly IntegrationTestFactory _factory;
    private readonly ApplicationDbContext _dbContext;

    public DatabaseIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Keep tests isolated (new test class instance per Fact, shared fixture)
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
    }

    #region User Repository Tests

    [Fact]
    public async Task UserRepository_CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new UserBuilder().Build();

        // Act
        var created = await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Assert
        created.Entity.Id.Should().BeGreaterThan(0);
        var retrieved = await _dbContext.Users.FindAsync(created.Entity.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task UserRepository_UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = new UserBuilder().Build();
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        user.FirstName = "Updated";
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _dbContext.Users.FindAsync(user.Id);
        retrieved.Should().NotBeNull();
        retrieved!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task UserRepository_DeleteAsync_ShouldDeleteUser()
    {
        // Arrange
        var user = new UserBuilder().Build();
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        var userId = user.Id;

        // Act
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _dbContext.Users.FindAsync(userId);
        retrieved.Should().BeNull();
    }

    #endregion

    #region OTP Repository Tests

    [Fact]
    public async Task OtpRepository_CreateAsync_ShouldCreateOtp()
    {
        // Arrange
        var otp = new OtpBuilder().Build();

        // Act
        await _dbContext.Otps.AddAsync(otp);
        await _dbContext.SaveChangesAsync();

        // Assert
        otp.Id.Should().BeGreaterThan(0);
        var retrieved = await _dbContext.Otps.FindAsync(otp.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Code.Should().Be("123456");
    }

    #endregion

    #region Transaction Tests

    [Fact]
    public async Task Database_TransactionRollback_ShouldNotPersistChanges()
    {
        // Arrange
        var user = new UserBuilder().Build();

        // Act
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            await transaction.RollbackAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
        }

        // Assert
        // Important: the same DbContext may still be tracking the entity even after rollback.
        _dbContext.ChangeTracker.Clear();
        var retrieved = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
        retrieved.Should().BeNull();
    }

    #endregion

    public void Dispose()
    {
        try
        {
            _dbContext.Database.EnsureDeleted();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}

