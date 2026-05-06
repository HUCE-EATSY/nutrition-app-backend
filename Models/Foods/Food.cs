namespace nutrition_app_backend.Models.Foods;

public class Food
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // Ảnh — null cho đến khi tích hợp Cloudinary (Giai đoạn 2)
    public string? ImageUrl { get; set; }

    // Dinh dưỡng trên 100g
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbPer100g { get; set; }
    public decimal FatPer100g { get; set; }

    // Ai tạo? null = dữ liệu hệ thống (admin seed)
    public Guid? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
