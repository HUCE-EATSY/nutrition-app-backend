using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Diaries;

using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Models.Foods;

public class FoodItem
{
    public Guid Id { get; set; }
    public string NameVi { get; set; } = null!;
    public string? NameEn { get; set; }
    public Guid? ParentId { get; set; }
    public byte CategoryId { get; set; }
    public FoodSource Source { get; set; } = FoodSource.Official;
    public FoodStatus Status { get; set; } = FoodStatus.Pending;
    public decimal ServingSizeG { get; set; }
    public string ServingUnitVi { get; set; } = "g";
    public string? ThumbnailUrl { get; set; }
    public ulong? ActiveImageId { get; set; }
    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    public string? Barcode { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public FoodItem? Parent { get; set; }
    public FoodCategory Category { get; set; } = null!;
    public User? Creator { get; set; }
    public FoodNutrition? Nutrition { get; set; }
    public FoodItemImage? ActiveImage { get; set; }
    
    public ICollection<FoodItem> Children { get; set; } = new List<FoodItem>();
    public ICollection<FoodItemImage> Images { get; set; } = new List<FoodItemImage>();
    public ICollection<FoodItemComponent> ComponentsAsParent { get; set; } = new List<FoodItemComponent>();
    public ICollection<FoodItemComponent> ComponentsAsChild { get; set; } = new List<FoodItemComponent>();
    public ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
    public ICollection<MenuFood> MenuFoods { get; set; } = new List<MenuFood>();
}
