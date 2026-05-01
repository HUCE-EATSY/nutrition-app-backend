using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Data;

public class WaoDbContext : DbContext
{
    public WaoDbContext(DbContextOptions<WaoDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<UserAuthProvider> UserAuthProviders { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Cấu hình bảng Users ---
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });

        // --- Cấu hình bảng UserAuthProviders ---
        modelBuilder.Entity<UserAuthProvider>(entity =>
        {
            entity.ToTable("user_auth_providers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.HasIndex(e => new { e.Provider, e.ProviderUid }).IsUnique();

            entity.HasOne(d => d.User)
                  .WithMany(p => p.AuthProviders)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Cấu hình bảng UserProfiles ---
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.HeightCm)
                .HasPrecision(5, 2);
            entity.Property(e => e.WeightKg)
                .HasPrecision(5, 2);
            
            entity.HasOne(d => d.User)
                  .WithOne(p => p.Profile)
                  .HasForeignKey<UserProfile>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Cấu hình bảng UserGoals ---
        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.ToTable("user_goals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.WeightKg)
                .HasPrecision(5, 2);
            entity.Property(e => e.GoalWeightKg)
                .HasPrecision(5, 2);
            entity.Property(e => e.BmrKcal)
                .HasPrecision(7, 2);
            entity.Property(e => e.TdeeKcal)
                .HasPrecision(7, 2);
            entity.Property(e => e.TargetCalories)
                .HasPrecision(7, 2);
            entity.Property(e => e.TargetProteinG)
                .HasPrecision(6, 2);
            entity.Property(e => e.TargetCarbsG)
                .HasPrecision(6, 2);
            entity.Property(e => e.TargetFatG)
                .HasPrecision(6, 2);

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Goals)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Cấu hình bảng RefreshTokens ---
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.HashedToken).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            
            entity.HasOne(d => d.User)
                  .WithMany(p => p.RefreshTokens)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}