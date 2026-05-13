namespace nutrition_app_backend.DTOs.Foods;

public record FoodResponse(
    Guid Id,
    string Name,
    string? ImageUrl,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal CarbPer100g,
    decimal FatPer100g,
    DateTime CreatedAt
);
