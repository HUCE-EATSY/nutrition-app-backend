namespace nutrition_app_backend.Models.Diary;

public class DiaryEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    // Lưu FoodId + FoodName để không cần JOIN mỗi lần query
    public Guid FoodId { get; set; }
    public string FoodName { get; set; } = string.Empty;

    public string DateISO { get; set; } = string.Empty; // "YYYY-MM-DD"
    public int Hour { get; set; }                        // 0-23

    public decimal QuantityG { get; set; }
    public decimal TotalCalories { get; set; }
    public decimal ProteinGram { get; set; }
    public decimal CarbGram { get; set; }
    public decimal FatGram { get; set; }

    public DateTime LoggedAt { get; set; }
}
