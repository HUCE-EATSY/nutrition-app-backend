namespace nutrition_app_backend.DTOs.Admin;

public class AdminFoodDto
{
    public Guid Id { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public byte CategoryId { get; set; }
    public byte Status { get; set; }
    public decimal ServingSizeG { get; set; }
    public string ServingUnitVi { get; set; } = "g";
    public string? ThumbnailUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public AdminNutritionDto? Nutrition { get; set; }
}
