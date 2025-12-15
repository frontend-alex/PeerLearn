using PeerLearn.Tests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Models.Auth;
using Infrastructure.Persistence.SQL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace PeerLearn.Tests.Common.Base;

/// <summary>
/// Base class for integration tests providing common setup and utilities
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestFactory>
{
    protected readonly IntegrationTestFactory Factory;
    protected readonly HttpClient Client;
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        // API serializes enums as strings (see API/Program.cs AddJsonOptions).
        Converters = { new JsonStringEnumConverter() }
    };

    protected BaseIntegrationTest(IntegrationTestFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected Task<T?> ReadJsonAsync<T>(HttpResponseMessage response)
        => response.Content.ReadFromJsonAsync<T>(JsonOptions);

    /// <summary>
    /// Registers a new user and returns the email
    /// </summary>
    protected async Task<string> RegisterUserAsync(string? email = null, string? username = null)
    {
        email ??= $"test_{Guid.NewGuid():N}@example.com";
        username ??= $"user_{Guid.NewGuid():N}";

        var request = new RegisterRequest
        {
            Username = username,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "Password123!"
        };

        await Client.PostAsJsonAsync("/api/auth/register", request);
        return email;
    }

    protected async Task VerifyUserEmailAsync(string email)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new InvalidOperationException($"Test user not found for email '{email}'.");
        }

        user.EmailVerified = true;
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Authenticates a user and sets the authorization header
    /// </summary>
    protected async Task<string> AuthenticateAsync(string? email = null, string? password = "Password123!")
    {
        email ??= await RegisterUserAsync();
        await VerifyUserEmailAsync(email);

        var loginRequest = new API.Models.Auth.LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        if (response.Headers.Contains("Set-Cookie"))
        {
            var cookies = response.Headers.GetValues("Set-Cookie");
            var tokenCookie = cookies.FirstOrDefault(c => c.StartsWith("access_token"));

            if (tokenCookie != null)
            {
                var parts = tokenCookie.Split(';');
                if (parts.Length > 0)
                {
                    var tokenParts = parts[0].Split('=');
                    if (tokenParts.Length > 1)
                    {
                        var token = tokenParts[1];
                        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        return token;
                    }
                }
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Sets authorization header with token
    /// </summary>
    protected void SetAuthHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}

