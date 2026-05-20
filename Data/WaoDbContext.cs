using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Models.Diary;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Diaries;


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
    public DbSet<Food> Foods { get; set; } = null!;
    public DbSet<FoodCategory> FoodCategories { get; set; } = null!;
    public DbSet<FoodItem> FoodItems { get; set; } = null!;
    public DbSet<FoodItemImage> FoodItemImages { get; set; } = null!;
    public DbSet<FoodNutrition> FoodNutritions { get; set; } = null!;
    public DbSet<FoodItemComponent> FoodItemComponents { get; set; } = null!;

    // Logging Group
    public DbSet<MealType> MealTypes { get; set; } = null!;
    public DbSet<FoodLog> FoodLogs { get; set; } = null!;

    // Streak & Subscription Group
    public DbSet<UserStreak> UserStreaks { get; set; } = null!;
    public DbSet<StreakFreezeTransaction> StreakFreezeTransactions { get; set; } = null!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
    public DbSet<Subscription> Subscriptions { get; set; } = null!;
    public DbSet<SubscriptionEvent> SubscriptionEvents { get; set; } = null!;

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
            
            entity.HasIndex(e => new { e.UserId, e.IsActive }).HasDatabaseName("idx_goals_user_active");
        });

        modelBuilder.Entity<WeightLog>(entity =>
        {
            entity.ToTable("weight_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.WeightKg).HasPrecision(5, 2);
            entity.HasIndex(e => new { e.UserId, e.LogDate }).IsUnique().HasDatabaseName("idx_weight_user_date");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.WeightLogs)
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
        modelBuilder.Entity<Food>(entity =>
        {
            entity.ToTable("foods");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Calories).HasPrecision(8, 2);
            entity.Property(e => e.Protein).HasPrecision(7, 2);
            entity.Property(e => e.Carbs).HasPrecision(7, 2);
            entity.Property(e => e.Fat).HasPrecision(7, 2);
            entity.Property(e => e.ServingSize).HasPrecision(7, 2);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.HasIndex(e => e.Category).HasDatabaseName("idx_food_category");
            entity.HasIndex(e => e.Name).HasDatabaseName("idx_food_name");
        });

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

        // --- GROUP 4: STREAKS & SUBSCRIPTIONS ---
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
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => new { e.UserId, e.ProtectedDate })
                  .IsUnique()
                  .HasDatabaseName("uq_freeze_date");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.StreakFreezeTransactions)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("subscription_plans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("subscriptions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.UserId).HasColumnType("CHAR(36)");
            entity.Property(e => e.PlanId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)");

            entity.HasIndex(e => new { e.UserId, e.Status }).HasDatabaseName("idx_sub_user_status");

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.Plan)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(d => d.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SubscriptionEvent>(entity =>
        {
            entity.ToTable("subscription_events");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnType("CHAR(36)");
            entity.Property(e => e.SubscriptionId).HasColumnType("CHAR(36)");
            entity.Property(e => e.ReceivedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            entity.HasIndex(e => new { e.SubscriptionId, e.ReceivedAt }).HasDatabaseName("idx_sub_event");

            entity.HasOne(d => d.Subscription)
                  .WithMany(p => p.Events)
                  .HasForeignKey(d => d.SubscriptionId)
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

        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan { Id = "free", Name = "Free Plan", DurationDays = 99999, Price = 0.00m },
            new SubscriptionPlan { Id = "premium_monthly", Name = "Premium Monthly", DurationDays = 30, Price = 9.99m },
            new SubscriptionPlan { Id = "premium_yearly", Name = "Premium Yearly", DurationDays = 365, Price = 99.99m }
        );
    }
}
