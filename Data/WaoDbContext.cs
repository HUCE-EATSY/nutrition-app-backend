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

    // Exercise Group
    public DbSet<ExerciseCategory> ExerciseCategories { get; set; } = null!;
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<ExerciseLog> ExerciseLogs { get; set; } = null!;

    // Notification Group
    public DbSet<NotificationType> NotificationTypes { get; set; } = null!;
    public DbSet<UserNotificationSetting> UserNotificationSettings { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<UserDeviceToken> UserDeviceTokens { get; set; } = null!;

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

        modelBuilder.Entity<UserDeviceToken>(entity =>
        {
            entity.ToTable("user_device_tokens");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => e.DeviceToken).IsUnique()
                  .HasDatabaseName("idx_user_device_token_unique");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.DeviceTokens)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
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
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), CategoryId = 1, NameVi = "Chạy bộ", NameEn = "Running", MetValue = 8.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/chay_bo_i0xdol.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), CategoryId = 1, NameVi = "Đi bộ", NameEn = "Walking", MetValue = 3.5m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/di_bo_zttbcb.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), CategoryId = 1, NameVi = "Đạp xe", NameEn = "Cycling", MetValue = 7.5m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/dap_xe_ydjkou.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), CategoryId = 1, NameVi = "Bơi lội", NameEn = "Swimming", MetValue = 9.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732740/boi_loi_ia9sol.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), CategoryId = 1, NameVi = "Nhảy dây", NameEn = "Jump Rope", MetValue = 12.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732743/nhay_day_deaept.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000014"), CategoryId = 1, NameVi = "Khiêu vũ", NameEn = "Dancing", MetValue = 4.5m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/khieu_vu_wpukmv.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000015"), CategoryId = 1, NameVi = "Aerobic", NameEn = "Aerobics", MetValue = 7.3m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732740/aerobic_dm9nsd.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000016"), CategoryId = 1, NameVi = "Leo núi", NameEn = "Climbing", MetValue = 8.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732743/leo_nui_im09ry.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Strength
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), CategoryId = 2, NameVi = "Tập tạ", NameEn = "Weight Training", MetValue = 6.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732743/nang_ta_miloiy.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), CategoryId = 2, NameVi = "Hít đất", NameEn = "Push-ups", MetValue = 8.0m, Unit = "reps", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/hit_dat_elcd4c.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), CategoryId = 2, NameVi = "Gập bụng", NameEn = "Sit-ups", MetValue = 8.0m, Unit = "reps", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/gap_bung_g2js1l.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000017"), CategoryId = 2, NameVi = "Kéo xà", NameEn = "Pull-ups", MetValue = 8.0m, Unit = "reps", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732743/keo_xa_vlicau.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000018"), CategoryId = 2, NameVi = "Squat", NameEn = "Squats", MetValue = 5.5m, Unit = "reps", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732741/squat_pcgt35.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000019"), CategoryId = 2, NameVi = "Plank", NameEn = "Plank", MetValue = 4.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732740/plank_k0rbjk.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Yoga & Flexibility
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), CategoryId = 3, NameVi = "Yoga", NameEn = "Yoga", MetValue = 3.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/yoga_bitlo9.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), CategoryId = 3, NameVi = "Pilates", NameEn = "Pilates", MetValue = 4.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/yoga_bitlo9.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Sports
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), CategoryId = 4, NameVi = "Bóng đá", NameEn = "Football/Soccer", MetValue = 10.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732741/bong_da_pcibi3.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), CategoryId = 4, NameVi = "Cầu lông", NameEn = "Badminton", MetValue = 7.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/cau_long_sxihz6.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000013"), CategoryId = 4, NameVi = "Bóng rổ", NameEn = "Basketball", MetValue = 8.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732741/bong_ro_zmldle.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000020"), CategoryId = 4, NameVi = "Lướt sóng", NameEn = "Surfing", MetValue = 5.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732743/luot_song_omzllo.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000021"), CategoryId = 4, NameVi = "Golf", NameEn = "Golf", MetValue = 4.5m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/golf_zj5sbo.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000022"), CategoryId = 4, NameVi = "Tennis", NameEn = "Tennis", MetValue = 7.3m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732742/tennis_pcqhh5.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000023"), CategoryId = 4, NameVi = "Trượt ván", NameEn = "Skateboarding", MetValue = 5.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732741/truot_van_rxlkn5.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000024"), CategoryId = 4, NameVi = "Bóng chày", NameEn = "Baseball", MetValue = 5.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732741/bong_chay_uiwjae.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000025"), CategoryId = 4, NameVi = "Bóng chuyền", NameEn = "Volleyball", MetValue = 4.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732741/bong_chuyen_v6us4w.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000026"), CategoryId = 4, NameVi = "Pickle Ball", NameEn = "Pickleball", MetValue = 4.5m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732740/pickle_ball_ciqj11.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Exercise { Id = Guid.Parse("10000000-0000-0000-0000-000000000027"), CategoryId = 4, NameVi = "Bóng bàn", NameEn = "Table Tennis", MetValue = 4.0m, Unit = "minutes", IconUrl = "https://res.cloudinary.com/drsgmoufr/image/upload/v1779732740/bong_ban_kgbyqe.jpg", Status = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
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
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }
}
