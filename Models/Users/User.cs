using nutrition_app_backend.Models.Diaries;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Models.Users;

public class User
{
    public Guid Id { get; set; }
    public byte Role { get; set; } = 1;
    public byte Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public ICollection<UserAuthProvider> AuthProviders { get; set; } = new List<UserAuthProvider>();
    public UserProfile? Profile { get; set; }
    public ICollection<UserGoal> Goals { get; set; } = new List<UserGoal>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public ICollection<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
    public ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
    public ICollection<FoodItem> CreatedFoods { get; set; } = new List<FoodItem>();

    public UserStreak? Streak { get; set; }
    public ICollection<StreakFreezeTransaction> StreakFreezeTransactions { get; set; } = new List<StreakFreezeTransaction>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}