namespace nutrition_app_backend.DTOs.Users;

public record UserGoalUpdateResponse(
    Guid Id,
    Guid UserId,
    decimal WeightKg,
    byte ActivityLevel,
    byte GoalType,
    decimal? GoalWeightKg,
    decimal BmrKcal,
    decimal TdeeKcal,
    decimal TargetCalories,
    decimal TargetProteinG,
    decimal TargetCarbsG,
    decimal TargetFatG,
    bool IsActive
);
