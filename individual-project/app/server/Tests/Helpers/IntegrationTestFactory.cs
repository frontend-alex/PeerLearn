using Infrastructure.Persistence.SQL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace PeerLearn.Tests.Helpers;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices((context, services) =>
        {
            // Remove ALL Entity Framework and DbContext-related service descriptors
            var descriptorsToRemove = services.Where(d =>
                d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ImplementationType == typeof(ApplicationDbContext)).ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Use an in-memory SQLite database for integration tests (self-contained, no external SQL Server needed)
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Add DbContext using SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    public new HttpClient CreateClient()
    {
        var client = base.CreateClient();
        
        // Reset database for each test instance
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }
        
        return client;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Clean up the database
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.EnsureDeleted();
                }
            }
            catch
            {
                // Ignore cleanup errors
            }

            try
            {
                _connection?.Dispose();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
        base.Dispose(disposing);
    }
}
