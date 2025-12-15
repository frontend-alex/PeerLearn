using Core.Models;

namespace PeerLearn.Tests.Common.Builders;

/// <summary>
/// Builder pattern for creating test User entities
/// </summary>
public class UserBuilder
{
    private User _user;

    public UserBuilder()
    {
        _user = new User
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            FirstName = "Test",
            LastName = "User",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            PasswordHash = "hashed_password",
            EmailVerified = false,
            Xp = 0
        };
    }

    public UserBuilder WithId(int id)
    {
        _user.Id = id;
        return this;
    }

    public UserBuilder WithUsername(string username)
    {
        _user.Username = username;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserBuilder WithName(string firstName, string lastName)
    {
        _user.FirstName = firstName;
        _user.LastName = lastName;
        return this;
    }

    public UserBuilder WithPasswordHash(string hash)
    {
        _user.PasswordHash = hash;
        return this;
    }

    public UserBuilder WithEmailVerified(bool verified = true)
    {
        _user.EmailVerified = verified;
        return this;
    }

    public UserBuilder WithXp(int xp)
    {
        _user.Xp = xp;
        return this;
    }

    public User Build() => _user;
}

