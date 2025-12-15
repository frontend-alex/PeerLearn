using DotNetEnv;
using API.Middleware;
using API.Setup.Swagger;
using API.Setup.Security;
using Core.Services.User;
using Core.Services.Auth;
using Core.Services.Workspace;
using Core.Services.Document;
using Infrastructure.Services;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Workspace;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.SQL;
using Core.Interfaces.Services;
using Core.Interfaces.repository.workspace;
using Core.Interfaces.repository.Document;

var builder = WebApplication.CreateBuilder(args);

// Only load .env file if not in Testing environment
if (builder.Environment.EnvironmentName != "Testing")
{
    Env.Load(".env");
}

// Controllers & JSON
builder.Services.AddControllers()
    .AddJsonOptions(o => {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.WriteIndented = true;
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
    
builder.Services.AddEndpointsApiExplorer();

// Security (CORS + Auth)
builder.Services.AddSecurityServices(builder.Configuration);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(
        Environment.GetEnvironmentVariable("CONNECTION_STRING")
        ?? builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings => 
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

Console.WriteLine("Using connection string: "
    + (Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IUserWorkspaceRepository, UserWorkspaceRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddSwaggerServices();

builder.Services.AddSignalR();

var app = builder.Build();

// Only use HTTPS redirection in production (not Development or Testing)
if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing")) {
    app.UseHttpsRedirection();
}

app.UseMiddleware<ErrorHandler>();

// Routing first
app.UseRouting();

// CORS must be between routing and auth
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Swagger after routing is fine
if (app.Environment.IsDevelopment()) {
    app.UseSwaggerServices(app.Environment);
}

app.MapHub<API.Hubs.AppHub>("/hubs/app");

// Endpoints
app.MapControllers();

// Apply migrations (skip for in-memory database or testing environment)
if (!app.Environment.IsEnvironment("Testing")) {
    using (var scope = app.Services.CreateScope()) {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // Only run migrations if not using in-memory database
        if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory") {
            context.Database.Migrate();
        }
    }
}

app.Run();

public partial class Program { }
