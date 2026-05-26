using System.Text.Json.Serialization;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Diaries;

public class StepLogResponse
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("log_date")]
    public DateOnly LogDate { get; set; }

    [JsonPropertyName("steps")]
    public int Steps { get; set; }

    [JsonPropertyName("step_goal")]
    public int StepGoal { get; set; }

    [JsonPropertyName("provider")]
    public HealthProvider? Provider { get; set; }

    [JsonPropertyName("calories_burned_kcal")]
    public decimal CaloriesBurnedKcal { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
