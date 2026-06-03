using System.ComponentModel.DataAnnotations;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Users;

public record UpdateProfileRequest(
    [MaxLength(100)]
    string? DisplayName,
    
    [MaxLength(500)]
    string? AvatarUrl,

    Gender Gender,
    DateOnly DateOfBirth,
    
    [Range(50, 300)]
    decimal HeightCm,
    
    [Range(20, 300)]
    decimal WeightKg,
    
    ActivityLevel ActivityLevel
);
