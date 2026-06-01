using System.ComponentModel.DataAnnotations;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Users;

public record OnboardingRequest(
    [Required, MaxLength(100)]
    string DisplayName,
    Gender Gender,
    DateOnly DateOfBirth,
    
    [Range(50, 300)]
    decimal HeightCm,
    
    [Range(20, 300)]
    decimal WeightKg,
    
    [Range(20, 300)]
    decimal GoalWeightKg,
    
    [Range(1, 5)]
    byte ActivityLevel,

    [Range(0.0, 2.0)]
    decimal WeeklyGoalKg,

    [Range(1, 3)]
    byte GoalType
);