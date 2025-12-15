using Core.Models;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.SQL;

public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
    }
  
    public DbSet<User> Users { get; set; }
    public DbSet<Otp> Otps { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<UserWorkspace> UserWorkspaces { get; set; }
    public DbSet<WorkspaceInvitation> WorkspaceInvitations { get; set; }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // NOTE: Our production database is SQL Server, but integration tests use SQLite in-memory.
        // SQLite has strict rules for AUTOINCREMENT: it must be on "INTEGER PRIMARY KEY".
        // When we force column types like "int", EF may emit "int PRIMARY KEY AUTOINCREMENT",
        // which SQLite rejects ("AUTOINCREMENT is only allowed on an INTEGER PRIMARY KEY").
        var isSqlite = Database.ProviderName?.Contains("Sqlite") == true;
        var intColumnType = isSqlite ? "INTEGER" : "int";
        var longTextColumnType = isSqlite ? "TEXT" : "nvarchar(max)";

        // Configure User entity
        modelBuilder.Entity<User>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType(intColumnType).ValueGeneratedOnAdd();
            entity.Property(e => e.Username).HasMaxLength(64);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(64);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(64);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(320);
            entity.Property(e => e.EmailVerified).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.Onboarding).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.ProfilePicture).HasMaxLength(2048);
            entity.Property(e => e.Xp).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Otp entity
        modelBuilder.Entity<Otp>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(320);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(6);
            entity.Property(e => e.ExpirationTime).IsRequired();

            entity.HasIndex(e => e.Email);
        });

        // Configure Workspace entity
        modelBuilder.Entity<Workspace>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType(intColumnType).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(128);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Visibility).IsRequired().HasConversion<string>();
            entity.Property(e => e.CreatedBy).IsRequired().HasColumnType(intColumnType);
            entity.Property(e => e.ColorHex).HasMaxLength(16);
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure UserWorkspace entity
        modelBuilder.Entity<UserWorkspace>(entity => {
            entity.HasKey(e => new { e.UserId, e.WorkspaceId });
            entity.Property(e => e.UserId).HasColumnType(intColumnType);
            entity.Property(e => e.WorkspaceId).HasColumnType(intColumnType);
            entity.Property(e => e.Role).IsRequired().HasConversion<string>();
            entity.Property(e => e.JoinedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Workspace)
                .WithMany(w => w.UserWorkspaces)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.WorkspaceId, e.UserId }).HasDatabaseName("IX_UserWorkspaces_Workspace_User");
        });

        // Configure WorkspaceInvitation entity
        modelBuilder.Entity<WorkspaceInvitation>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType(intColumnType).ValueGeneratedOnAdd();
            entity.Property(e => e.WorkspaceId).HasColumnType(intColumnType);
            entity.Property(e => e.InvitedBy).HasColumnType(intColumnType);
            entity.Property(e => e.InvitedEmail).IsRequired().HasMaxLength(320);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Workspace)
                .WithMany(w => w.Invitations)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Inviter)
                .WithMany()
                .HasForeignKey(e => e.InvitedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Token).IsUnique();
        });

        // Configure Document entity
        modelBuilder.Entity<Document>(entity => {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType(intColumnType).ValueGeneratedOnAdd();
            entity.Property(e => e.WorkspaceId).HasColumnType(intColumnType);
            entity.Property(e => e.CreatedBy).HasColumnType(intColumnType);
            entity.Property(e => e.Title).HasMaxLength(256);
            entity.Property(e => e.Kind).IsRequired().HasConversion<string>();
            entity.Property(e => e.YDocId).IsRequired().HasMaxLength(128);
            entity.Property(e => e.ColorHex).HasMaxLength(16);
            entity.Property(e => e.Content).HasColumnType(longTextColumnType);
            entity.Property(e => e.IsArchived).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.Visibility).IsRequired().HasConversion<string>();
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasConversion(
                    v => v.ToUniversalTime(), // Store: ensure UTC
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Read: mark as UTC (database stores UTC)
                );
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null, // Store: ensure UTC
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null // Read: mark as UTC (database stores UTC)
                );

            entity.HasOne(e => e.Workspace)
                .WithMany(w => w.Documents)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.YDocId).IsUnique();
        });
    }
}