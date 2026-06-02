using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Models.Diaries;
using nutrition_app_backend.Models.Exercises;
using nutrition_app_backend.Models.Notifications;
using nutrition_app_backend.Extensions;

namespace nutrition_app_backend.Data;

public class WaoDbContext : DbContext
{
    public WaoDbContext(DbContextOptions<WaoDbContext> options) : base(options) { }

    // User Group
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserAuthProvider> UserAuthProviders { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<UserGoal> UserGoals { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<WeightLog> WeightLogs { get; set; } = null!;

    // Phase 3 Group
    public DbSet<UserStreak> UserStreaks { get; set; } = null!;
    public DbSet<StreakFreezeTransaction> StreakFreezeTransactions { get; set; } = null!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<SubscriptionEvent> SubscriptionEvents { get; set; } = null!;

    // Food Group
    public DbSet<FoodCategory> FoodCategories { get; set; } = null!;
    public DbSet<FoodItem> FoodItems { get; set; } = null!;
    public DbSet<FoodItemImage> FoodItemImages { get; set; } = null!;
    public DbSet<FoodNutrition> FoodNutritions { get; set; } = null!;
    public DbSet<FoodItemComponent> FoodItemComponents { get; set; } = null!;

    // Logging Group
    public DbSet<MealType> MealTypes { get; set; } = null!;
    public DbSet<FoodLog> FoodLogs { get; set; } = null!;
    public DbSet<StepLog> StepLogs { get; set; } = null!;
    public DbSet<UserHealthConnection> UserHealthConnections { get; set; } = null!;

    // Menus & Daily Plans Group
    public DbSet<Menu> Menus { get; set; } = null!;
    public DbSet<MenuFood> MenuFoods { get; set; } = null!;
    public DbSet<DailyPlan> DailyPlans { get; set; } = null!;

    // Exercise Group
    public DbSet<ExerciseCategory> ExerciseCategories { get; set; } = null!;
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<ExerciseLog> ExerciseLogs { get; set; } = null!;

    // Notification Group
    public DbSet<NotificationType> NotificationTypes { get; set; } = null!;
    public DbSet<UserNotificationSetting> UserNotificationSettings { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- GROUP 1: USER & AUTHENTICATION ---
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
        });

        modelBuilder.Entity<UserAuthProvider>(entity =>
        {
            entity.ToTable("user_auth_providers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.HashedPassword)
                  .HasColumnName("hashed_password")
                  .HasMaxLength(255)
                  .IsRequired(false);
            entity.HasIndex(e => new { e.Provider, e.ProviderUid }).IsUnique();

            entity.HasOne(d => d.User)
                  .WithMany(p => p.AuthProviders)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.HeightCm).HasPrecision(5, 2);
            entity.Property(e => e.WeightKg).HasPrecision(5, 2);
            
            entity.HasOne(d => d.User)
                  .WithOne(p => p.Profile)
                  .HasForeignKey<UserProfile>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.ToTable("user_goals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.WeightKg).HasPrecision(5, 2);
            entity.Property(e => e.GoalWeightKg).HasPrecision(5, 2);
            entity.Property(e => e.WeeklyGoalKg).HasPrecision(3, 2);
            entity.Property(e => e.BmrKcal).HasPrecision(7, 2);
            entity.Property(e => e.TdeeKcal).HasPrecision(7, 2);
            entity.Property(e => e.TargetCalories).HasPrecision(7, 2);
            entity.Property(e => e.TargetProteinG).HasPrecision(6, 2);
            entity.Property(e => e.TargetCarbsG).HasPrecision(6, 2);
            entity.Property(e => e.TargetFatG).HasPrecision(6, 2);

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Goals)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WeightLog>(entity =>
        {
            entity.ToTable("weight_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.WeightKg).HasPrecision(5, 2);
            entity.Property(e => e.PhotoUrl).HasMaxLength(2048).IsRequired(false);
            // Chỉ cần 1 Index Unique là đủ cho cả query và tính duy nhất
            entity.HasIndex(e => new { e.UserId, e.LogDate }).IsUnique().HasDatabaseName("idx_weight_user_date");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.WeightLogs)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StepLog>(entity =>
        {
            entity.ToTable("step_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CaloriesBurnedKcal).HasPrecision(8, 2);
            entity.HasIndex(e => new { e.UserId, e.LogDate }).IsUnique().HasDatabaseName("idx_steps_user_date");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.StepLogs)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserHealthConnection>(entity =>
        {
            entity.ToTable("user_health_connections");
            entity.HasKey(e => new { e.UserId, e.Provider });
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.HealthConnections)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

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

        // --- PHASE 3: STREAK & SUBSCRIPTION ---
        modelBuilder.Entity<UserStreak>(entity =>
        {
            entity.ToTable("user_streaks");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");

            entity.HasOne(d => d.User)
                  .WithOne(p => p.Streak)
                  .HasForeignKey<UserStreak>(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StreakFreezeTransaction>(entity =>
        {
            entity.ToTable("streak_freeze_transactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");

            entity.HasIndex(e => new { e.UserId, e.FreezeDate }).IsUnique().HasDatabaseName("idx_freeze_user_date");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.StreakFreezeTransactions)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("subscription_plans");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("subscriptions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            
            entity.HasIndex(e => new { e.UserId, e.Status }).HasDatabaseName("idx_sub_user_status");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Plan)
                  .WithMany()
                  .HasForeignKey(d => d.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SubscriptionEvent>(entity =>
        {
            entity.ToTable("subscription_events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.SubscriptionId).HasColumnType("CHAR(36)");

            entity.HasIndex(e => new { e.SubscriptionId, e.ReceivedAt }).HasDatabaseName("idx_sub_event");

            entity.HasOne(d => d.Subscription)
                  .WithMany()
                  .HasForeignKey(d => d.SubscriptionId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // --- GROUP 2: FOOD DATABASE ---
        modelBuilder.Entity<FoodCategory>(entity =>
        {
            entity.ToTable("food_categories");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.ToTable("food_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.ParentId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CreatedBy).HasColumnType("CHAR(36)");
            entity.Property(e => e.ServingSizeG).HasPrecision(8, 2);
            
            entity.HasIndex(e => new { e.NameVi, e.NameEn }).IsFullText().HasDatabaseName("idx_food_ft");
            entity.HasIndex(e => e.ActiveImageId).HasDatabaseName("idx_food_active_image");
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_food_status");
            entity.HasIndex(e => new { e.CreatedAt, e.Id }).HasDatabaseName("idx_food_cursor_pagination");
            
            entity.HasOne(d => d.Parent)
                  .WithMany(p => p.Children)
                  .HasForeignKey(d => d.ParentId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Category)
                  .WithMany(p => p.FoodItems)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Creator)
                  .WithMany(p => p.CreatedFoods)
                  .HasForeignKey(d => d.CreatedBy)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.ActiveImage)
                  .WithOne()
                  .HasForeignKey<FoodItem>(d => d.ActiveImageId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FoodItemImage>(entity =>
        {
            entity.ToTable("food_item_images");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FoodItemId).HasColumnType("CHAR(36)");
            entity.HasIndex(e => e.FoodItemId).HasDatabaseName("idx_img_food");

            entity.HasOne(d => d.FoodItem)
                  .WithMany(p => p.Images)
                  .HasForeignKey(d => d.FoodItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FoodNutrition>(entity =>
        {
            entity.ToTable("food_nutrition");
            entity.HasKey(e => e.FoodItemId);
            entity.Property(e => e.FoodItemId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CaloriesKcal).HasPrecision(8, 2);
            entity.Property(e => e.ProteinG).HasPrecision(7, 2);
            entity.Property(e => e.CarbsG).HasPrecision(7, 2);
            entity.Property(e => e.FatG).HasPrecision(7, 2);
            entity.Property(e => e.FiberG).HasPrecision(7, 2);
            entity.Property(e => e.SugarG).HasPrecision(7, 2);
            entity.Property(e => e.SodiumMg).HasPrecision(8, 2);

            entity.HasOne(d => d.FoodItem)
                  .WithOne(p => p.Nutrition)
                  .HasForeignKey<FoodNutrition>(d => d.FoodItemId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FoodItemComponent>(entity =>
        {
            entity.ToTable("food_item_components");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ParentFoodId).HasColumnType("CHAR(36)");
            entity.Property(e => e.ChildFoodId).HasColumnType("CHAR(36)");
            entity.Property(e => e.QuantityG).HasPrecision(8, 2);
            entity.HasIndex(e => new { e.ParentFoodId, e.ChildFoodId }).IsUnique();

            entity.HasOne(d => d.ParentFood)
                  .WithMany(p => p.ComponentsAsParent)
                  .HasForeignKey(d => d.ParentFoodId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.ChildFood)
                  .WithMany(p => p.ComponentsAsChild)
                  .HasForeignKey(d => d.ChildFoodId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- GROUP 3: FOOD LOGGING ---
        modelBuilder.Entity<MealType>(entity =>
        {
            entity.ToTable("meal_types");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<FoodLog>(entity =>
        {
            entity.ToTable("food_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.FoodItemId).HasColumnType("CHAR(36)");
            entity.Property(e => e.QuantityG).HasPrecision(8, 2);
            entity.Property(e => e.CaloriesKcal).HasPrecision(8, 2);
            entity.Property(e => e.ProteinG).HasPrecision(7, 2);
            entity.Property(e => e.CarbsG).HasPrecision(7, 2);
            entity.Property(e => e.FatG).HasPrecision(7, 2);

            entity.HasOne(d => d.User)
                  .WithMany(p => p.FoodLogs)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Chỉ dùng 1 index phức hợp duy nhất bắt đầu bằng UserId để vừa hỗ trợ query vừa hỗ trợ Foreign Key
            entity.HasIndex(e => new { e.UserId, e.LogDate })
                  .HasDatabaseName("idx_logs_user_date");

            entity.HasOne(d => d.FoodItem)
                  .WithMany(p => p.FoodLogs)
                  .HasForeignKey(d => d.FoodItemId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.MealType)
                  .WithMany(p => p.FoodLogs)
                  .HasForeignKey(d => d.MealTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- GROUP 4: EXERCISE ---
        modelBuilder.Entity<ExerciseCategory>(entity =>
        {
            entity.ToTable("exercise_categories");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.ToTable("exercises");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.MetValue).HasPrecision(5, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");
            
            entity.HasIndex(e => e.Status).HasDatabaseName("idx_exercise_status");
            entity.HasIndex(e => e.CategoryId).HasDatabaseName("idx_exercise_category");

            entity.HasOne(d => d.Category)
                  .WithMany(p => p.Exercises)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ExerciseLog>(entity =>
        {
            entity.ToTable("exercise_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.ExerciseId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CaloriesBurned).HasPrecision(8, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => new { e.UserId, e.LogDate })
                  .HasDatabaseName("idx_exercise_logs_user_date");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.ExerciseLogs)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Exercise)
                  .WithMany(p => p.ExerciseLogs)
                  .HasForeignKey(d => d.ExerciseId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- GROUP 5: NOTIFICATION ---
        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.ToTable("notification_types");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<UserNotificationSetting>(entity =>
        {
            entity.ToTable("user_notification_settings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => new { e.UserId, e.NotificationTypeId })
                  .IsUnique()
                  .HasDatabaseName("idx_user_notif_setting_unique");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.NotificationSettings)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.NotificationType)
                  .WithMany(p => p.UserSettings)
                  .HasForeignKey(d => d.NotificationTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                  .HasDatabaseName("idx_notif_user_created");
            entity.HasIndex(e => new { e.UserId, e.IsRead })
                  .HasDatabaseName("idx_notif_user_read");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Notifications)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.NotificationType)
                  .WithMany(p => p.Notifications)
                  .HasForeignKey(d => d.NotificationTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- SEED DATA ---
        modelBuilder.Entity<FoodCategory>().HasData(
            new FoodCategory { Id = 1, NameVi = "Cơm & Xôi", NameEn = "Rice dishes" },
            new FoodCategory { Id = 2, NameVi = "Phở & Bún", NameEn = "Noodle soups" },
            new FoodCategory { Id = 3, NameVi = "Bánh mì & Bánh", NameEn = "Bread & Pastries" },
            new FoodCategory { Id = 4, NameVi = "Đồ uống", NameEn = "Beverages" },
            new FoodCategory { Id = 5, NameVi = "Thực phẩm đóng gói", NameEn = "Packaged food" },
            new FoodCategory { Id = 6, NameVi = "Rau củ quả", NameEn = "Vegetables & Fruits" },
            new FoodCategory { Id = 7, NameVi = "Thịt & Hải sản", NameEn = "Meat & Seafood" },
            new FoodCategory { Id = 8, NameVi = "Chuỗi F&B", NameEn = "F&B Chains" },
            new FoodCategory { Id = 9, NameVi = "Quốc tế", NameEn = "International" },
            new FoodCategory { Id = 10, NameVi = "Khác", NameEn = "Other" }
        );

        modelBuilder.Entity<MealType>().HasData(
            new MealType { Id = 1, NameVi = "Bữa sáng" },
            new MealType { Id = 2, NameVi = "Bữa trưa" },
            new MealType { Id = 3, NameVi = "Bữa tối" },
            new MealType { Id = 4, NameVi = "Bữa phụ" }
        );

        modelBuilder.Entity<ExerciseCategory>().HasData(
            new ExerciseCategory { Id = 1, NameVi = "Cardio", NameEn = "Cardio", DisplayOrder = 1 },
            new ExerciseCategory { Id = 2, NameVi = "Sức mạnh", NameEn = "Strength", DisplayOrder = 2 },
            new ExerciseCategory { Id = 3, NameVi = "Yoga & Pilates", NameEn = "Yoga & Pilates", DisplayOrder = 3 },
            new ExerciseCategory { Id = 4, NameVi = "Thể thao", NameEn = "Sports", DisplayOrder = 4 },
            new ExerciseCategory { Id = 5, NameVi = "Khác", NameEn = "Other", DisplayOrder = 5 }
        );

        modelBuilder.Entity<Exercise>().HasData(
            // Cardio
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), CategoryId = 1, NameVi = "Chạy bộ", NameEn = "Running", MetValue = 8.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), CategoryId = 1, NameVi = "Đi bộ", NameEn = "Walking", MetValue = 3.5m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), CategoryId = 1, NameVi = "Đạp xe", NameEn = "Cycling", MetValue = 7.5m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), CategoryId = 1, NameVi = "Bơi lội", NameEn = "Swimming", MetValue = 9.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), CategoryId = 1, NameVi = "Nhảy dây", NameEn = "Jump Rope", MetValue = 12.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Strength
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), CategoryId = 2, NameVi = "Tập tạ", NameEn = "Weight Training", MetValue = 6.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), CategoryId = 2, NameVi = "Hít đất", NameEn = "Push-ups", MetValue = 8.0m, Unit = "reps", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), CategoryId = 2, NameVi = "Gập bụng", NameEn = "Sit-ups", MetValue = 8.0m, Unit = "reps", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Yoga
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), CategoryId = 3, NameVi = "Yoga", NameEn = "Yoga", MetValue = 3.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), CategoryId = 3, NameVi = "Pilates", NameEn = "Pilates", MetValue = 4.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Sports
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), CategoryId = 4, NameVi = "Bóng đá", NameEn = "Football/Soccer", MetValue = 10.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), CategoryId = 4, NameVi = "Cầu lông", NameEn = "Badminton", MetValue = 7.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000013"), CategoryId = 4, NameVi = "Bóng rổ", NameEn = "Basketball", MetValue = 8.0m, Unit = "minutes", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<NotificationType>().HasData(
            new NotificationType { Id = 1, Code = "MEAL_REMINDER", NameVi = "Nhắc nhở bữa ăn", NameEn = "Meal Reminder", Description = "Nhắc nhở ghi nhật ký bữa ăn" },
            new NotificationType { Id = 2, Code = "EXERCISE_REMINDER", NameVi = "Nhắc nhở tập luyện", NameEn = "Exercise Reminder", Description = "Nhắc nhở ghi nhật ký tập luyện" },
            new NotificationType { Id = 3, Code = "WEIGHT_LOG_REMINDER", NameVi = "Nhắc nhở cân nặng", NameEn = "Weight Log Reminder", Description = "Nhắc nhở ghi lại cân nặng" },
            new NotificationType { Id = 4, Code = "WATER_REMINDER", NameVi = "Nhắc nhở uống nước", NameEn = "Water Reminder", Description = "Nhắc nhở uống nước" },
            new NotificationType { Id = 5, Code = "GOAL_ACHIEVED", NameVi = "Đạt mục tiêu", NameEn = "Goal Achieved", Description = "Thông báo khi đạt mục tiêu" },
            new NotificationType { Id = 6, Code = "DAILY_SUMMARY", NameVi = "Tổng kết ngày", NameEn = "Daily Summary", Description = "Tổng kết dinh dưỡng và tập luyện trong ngày" },
            new NotificationType { Id = 7, Code = "WEEKLY_REPORT", NameVi = "Báo cáo tuần", NameEn = "Weekly Report", Description = "Báo cáo tiến độ hàng tuần" }
        );

        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan { Id = 1, Code = "FREE", Name = "Gói Miễn Phí", Price = 0, DurationDays = 99999, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new SubscriptionPlan { Id = 2, Code = "MONTHLY_PREMIUM", Name = "Premium 1 Tháng", Price = 59000, DurationDays = 30, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) },
            new SubscriptionPlan { Id = 3, Code = "YEARLY_PREMIUM", Name = "Premium 1 Năm", Price = 499000, DurationDays = 365, CreatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc), UpdatedAt = new DateTime(2024,1,1,0,0,0,DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("menus");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.TotalCalories).HasPrecision(18, 2);
            entity.Property(e => e.TotalProtein).HasPrecision(18, 2);
            entity.Property(e => e.TotalCarbs).HasPrecision(18, 2);
            entity.Property(e => e.TotalFat).HasPrecision(18, 2);

            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MenuFood>(entity =>
        {
            entity.ToTable("menu_foods");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.MenuId).HasColumnType("CHAR(36)");
            entity.Property(e => e.FoodItemId).HasColumnType("CHAR(36)");
            entity.Property(e => e.QuantityG).HasPrecision(8, 2);

            entity.HasOne(d => d.Menu)
                  .WithMany(p => p.MenuFoods)
                  .HasForeignKey(d => d.MenuId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.FoodItem)
                  .WithMany()
                  .HasForeignKey(d => d.FoodItemId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.MealType)
                  .WithMany()
                  .HasForeignKey(d => d.MealTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DailyPlan>(entity =>
        {
            entity.ToTable("daily_plans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.FoodItemId).HasColumnType("CHAR(36)");
            entity.Property(e => e.QuantityG).HasPrecision(8, 2);

            entity.HasIndex(e => new { e.UserId, e.LogDate })
                  .HasDatabaseName("idx_daily_plan_user_date");

            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.FoodItem)
                  .WithMany()
                  .HasForeignKey(d => d.FoodItemId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.MealType)
                  .WithMany()
                  .HasForeignKey(d => d.MealTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }
}
