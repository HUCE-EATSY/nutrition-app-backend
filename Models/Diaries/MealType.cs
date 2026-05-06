namespace nutrition_app_backend.Models.Diaries;

public class MealType
{
    public byte Id { get; set; }
    public string NameVi { get; set; } = null!;

    public ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
}
