<<<<<<< HEAD
=======
using System.ComponentModel.DataAnnotations;

>>>>>>> feature/phase-2-food-db-and-logging
namespace nutrition_app_backend.Models.Foods;

public class Food
{
<<<<<<< HEAD
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
=======
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>Calories per serving (kcal)</summary>
    public float Calories { get; set; }

    /// <summary>Protein per serving (g)</summary>
    public float Protein { get; set; }

    /// <summary>Carbohydrates per serving (g)</summary>
    public float Carbs { get; set; }

    /// <summary>Fat per serving (g)</summary>
    public float Fat { get; set; }

    /// <summary>Serving size in grams</summary>
    public float ServingSize { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
>>>>>>> feature/phase-2-food-db-and-logging
}
