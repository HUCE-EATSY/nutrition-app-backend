namespace nutrition_app_backend.Models.Foods;

public class FoodItemComponent
{
    public ulong Id { get; set; }
    public Guid ParentFoodId { get; set; }
    public Guid ChildFoodId { get; set; }
    public decimal QuantityG { get; set; }

    public FoodItem ParentFood { get; set; } = null!;
    public FoodItem ChildFood { get; set; } = null!;
}
