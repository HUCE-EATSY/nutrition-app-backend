namespace nutrition_app_backend.Models.Foods;

public class FoodCategory
{
    public byte Id { get; set; }
    public string NameVi { get; set; } = null!;
    public string? NameEn { get; set; }

    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
