using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class UpdateFoodLogRequest
{
    [Required]
    [Range(0.01, 99999.99)]
    [JsonPropertyName("quantity_g")]
    public decimal QuantityG { get; set; }
}
