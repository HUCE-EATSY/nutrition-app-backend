using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Foods;

/// <summary>
/// Request body để tạo món ăn mới.
/// Giai đoạn 1: ImageUrl là URL tùy chọn (người dùng paste link).
/// Giai đoạn 2: Sẽ thêm IFormFile để upload lên Cloudinary.
/// </summary>
public record CreateFoodRequest(
    [Required]
    [MaxLength(200)]
    string Name,

    [Range(0, 9999.99)]
    decimal CaloriesPer100g,

    [Range(0, 999.99)]
    decimal ProteinPer100g,

    [Range(0, 999.99)]
    decimal CarbPer100g,

    [Range(0, 999.99)]
    decimal FatPer100g,

    // Giai đoạn 1: tạm thời nhận URL ảnh (có thể null)
    string? ImageUrl = null
);
