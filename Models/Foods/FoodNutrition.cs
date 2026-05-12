namespace nutrition_app_backend.Models.Foods;

public class FoodNutrition
{
    public Guid FoodItemId { get; set; }
    public decimal CaloriesKcal { get; set; }
    public decimal ProteinG { get; set; } = 0;
    public decimal CarbsG { get; set; } = 0;
    public decimal FatG { get; set; } = 0;
    public decimal? FiberG { get; set; }
    public decimal? SugarG { get; set; }
    public decimal? SodiumMg { get; set; }
    public DateTime UpdatedAt { get; set; }

    public FoodItem FoodItem { get; set; } = null!;
}
