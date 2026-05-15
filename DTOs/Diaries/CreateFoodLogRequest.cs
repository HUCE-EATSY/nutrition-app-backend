using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class CreateFoodLogRequest
{
    [Required]
    [JsonPropertyName("food_item_id")]
    public Guid FoodItemId { get; set; }

    [Required]
    [Range(1, 255)]
    [JsonPropertyName("meal_type_id")]
    public byte MealTypeId { get; set; }

    [Required]
    [JsonPropertyName("log_date")]
    public DateOnly LogDate { get; set; }

    [Required]
    [Range(0.01, 99999.99)]
    [JsonPropertyName("quantity_g")]
    public decimal QuantityG { get; set; }

    [Range(1, 10)]
    [JsonPropertyName("input_method")]
    public byte InputMethod { get; set; } = 5;

    [MaxLength(500)]
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}
