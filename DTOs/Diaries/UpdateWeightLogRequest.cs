using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class UpdateWeightLogRequest
{
    [Required]
    [Range(1, 500)]
    [JsonPropertyName("weight_kg")]
    public decimal WeightKg { get; set; }

    [MaxLength(500)]
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}
