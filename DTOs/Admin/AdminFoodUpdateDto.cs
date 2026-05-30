namespace nutrition_app_backend.DTOs.Admin;

public class AdminFoodUpdateDto
{
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public byte? CategoryId { get; set; }
    public decimal? ServingSizeG { get; set; }
    public string? ServingUnitVi { get; set; }
    public string? ThumbnailUrl { get; set; }
    
    public AdminNutritionDto? Nutrition { get; set; }
}
