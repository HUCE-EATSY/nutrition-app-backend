using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Diaries;

public class WeightLogResponse
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("weight_kg")]
    public decimal WeightKg { get; set; }

    [JsonPropertyName("log_date")]
    public DateOnly LogDate { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}
