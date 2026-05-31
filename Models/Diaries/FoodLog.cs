using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Models.Diaries;

public class FoodLog
{
    public ulong Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FoodItemId { get; set; }
    public byte MealTypeId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal QuantityG { get; set; }
    public decimal CaloriesKcal { get; set; }
    public decimal ProteinG { get; set; } = 0;
    public decimal CarbsG { get; set; } = 0;
    public decimal FatG { get; set; } = 0;
    public byte InputMethod { get; set; } = 5;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public FoodItem FoodItem { get; set; } = null!;
    public MealType MealType { get; set; } = null!;
}
