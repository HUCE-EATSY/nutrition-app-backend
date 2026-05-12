namespace nutrition_app_backend.Models.Foods;

public class FoodItemImage
{
    public ulong Id { get; set; }
    public Guid FoodItemId { get; set; }
    public string StoragePath { get; set; } = null!;
    public string StorageProvider { get; set; } = "gcs";
    public DateTime CreatedAt { get; set; }

    public FoodItem FoodItem { get; set; } = null!;
}
