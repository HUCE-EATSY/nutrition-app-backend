using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Diaries;

public class UpsertStepLogRequest
{
    [Required]
    [JsonPropertyName("log_date")]
    public DateOnly LogDate { get; set; }

    [Required]
    [Range(0, 200000)]
    [JsonPropertyName("steps")]
    public int Steps { get; set; }

    [Required]
    [Range(1, 200000)]
    [JsonPropertyName("step_goal")]
    public int StepGoal { get; set; }

    [JsonPropertyName("provider")]
    public HealthProvider? Provider { get; set; }

    [JsonPropertyName("calories_burned_kcal")]
    public decimal? CaloriesBurnedKcal { get; set; }
}
