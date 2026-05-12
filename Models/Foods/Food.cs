using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.Models.Foods;

public class Food
{
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
}
