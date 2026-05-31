using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Foods;

public class CreateMenuRequest
{
    [Required(ErrorMessage = "Tên thực đơn không được để trống")]
    [MaxLength(255)]
    public string Name { get; set; } = null!;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class UpdateMenuRequest
{
    [Required(ErrorMessage = "Tên thực đơn không được để trống")]
    [MaxLength(255)]
    public string Name { get; set; } = null!;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class AddFoodToMenuRequest
{
    [Required]
    public Guid FoodItemId { get; set; }
    
    [Range(0.1, 10000, ErrorMessage = "Khối lượng phải lớn hơn 0")]
    public decimal QuantityG { get; set; }
}

public class MenuResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Macro sums and percentages
    public decimal TotalCalories { get; set; }
    public decimal TotalProteinG { get; set; }
    public decimal TotalCarbsG { get; set; }
    public decimal TotalFatG { get; set; }
    
    public decimal ProteinPercentage { get; set; }
    public decimal CarbsPercentage { get; set; }
    public decimal FatPercentage { get; set; }

    public List<MenuFoodResponse> Foods { get; set; } = new List<MenuFoodResponse>();
}

public class MenuFoodResponse
{
    public Guid Id { get; set; }
    public Guid FoodItemId { get; set; }
    public string FoodNameVi { get; set; } = null!;
    public string? FoodNameEn { get; set; }
    public string? ThumbnailUrl { get; set; }
    
    public decimal QuantityG { get; set; }
    
    // Calculated macros for this specific quantity
    public decimal CaloriesKcal { get; set; }
    public decimal ProteinG { get; set; }
    public decimal CarbsG { get; set; }
    public decimal FatG { get; set; }
}
