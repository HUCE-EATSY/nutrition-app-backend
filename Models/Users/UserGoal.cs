namespace nutrition_app_backend.Models.Users;

public class UserGoal
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal WeightKg { get; set; }
    public byte ActivityLevel { get; set; } = 1;
    public byte GoalType { get; set; } = 3;
    public decimal? GoalWeightKg { get; set; }
    public decimal BmrKcal { get; set; }
    public decimal TdeeKcal { get; set; }
    public decimal TargetCalories { get; set; }
    public decimal TargetProteinG { get; set; }
    public decimal TargetCarbsG { get; set; }
    public decimal TargetFatG { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}