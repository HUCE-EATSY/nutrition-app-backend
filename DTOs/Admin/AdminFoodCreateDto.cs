using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Admin;

public class AdminFoodCreateDto
{
    [Required]
    public string NameVi { get; set; } = string.Empty;
    
    public string? NameEn { get; set; }
    
    [Required]
    public byte CategoryId { get; set; }
    
    [Required]
    public decimal ServingSizeG { get; set; }
    
    public string ServingUnitVi { get; set; } = "g";
    
    public string? ThumbnailUrl { get; set; }
    
    [Required]
    public AdminNutritionDto Nutrition { get; set; } = null!;
}
