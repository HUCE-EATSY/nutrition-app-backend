using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Users;

public record UpdateUserGoalRequest(
    byte GoalType,
    decimal? GoalWeightKg,
    
    [Range(100, 5000)]
    decimal TargetCalories,
    
    [Range(0, 500)]
    decimal TargetProteinG,
    
    [Range(0, 500)]
    decimal TargetCarbsG,
    
    [Range(0, 300)]
    decimal TargetFatG
);
