using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Users;

public record UserProfileResponse(
    Guid UserId,
    Gender Gender,
    DateOnly DateOfBirth,
    decimal HeightCm,
    decimal WeightKg,
    DateTime UpdatedAt
);
