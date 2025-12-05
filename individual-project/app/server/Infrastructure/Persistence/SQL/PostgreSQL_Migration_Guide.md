# PostgreSQL Migration Guide

This guide helps you migrate from SQL Server to PostgreSQL for Raspberry Pi deployment.

## Step 1: Update Package References

In `app/server/Infrastructure/Infrastructure.csproj`:

**Remove:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
```

**Add:**
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.2" />
```

## Step 2: Update Program.cs

In `app/server/API/Program.cs`, change:

```csharp
// OLD:
options.UseSqlServer(
    Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection"));

// NEW:
options.UseNpgsql(
    Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection"));
```

## Step 3: Update Connection String

In `.env` or `appsettings.json`:

**SQL Server format:**
```
Server=localhost;Database=PeerLearnDb;User Id=user;Password=pass;
```

**PostgreSQL format:**
```
Host=localhost;Database=peerlearndb;Username=peerlearnuser;Password=your_password
```

## Step 4: Update ApplicationDbContext

Some SQL Server-specific syntax may need changes:

1. **Default values**: PostgreSQL uses `DEFAULT` instead of `CURRENT_TIMESTAMP`
2. **Identity columns**: Use `ValueGeneratedOnAdd()` (works the same)
3. **String length**: PostgreSQL handles this differently

Example changes in `ApplicationDbContext.cs`:

```csharp
// Change from:
entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

// To:
entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
```

## Step 5: Create New Migration

After making changes:

```bash
cd app/server/API
dotnet ef migrations add PostgresMigration
dotnet ef database update
```

## Step 6: Test Locally First

Before deploying to Raspberry Pi, test PostgreSQL locally:

1. Install PostgreSQL on your dev machine
2. Update connection string
3. Run migrations
4. Test all functionality

## Common Issues

1. **Case sensitivity**: PostgreSQL is case-sensitive. Use lowercase table/column names or quote them
2. **DateTime**: Use `timestamp` instead of `datetime2`
3. **String concatenation**: Use `||` instead of `+`
4. **Identity**: Use `SERIAL` or `BIGSERIAL` instead of `IDENTITY`


