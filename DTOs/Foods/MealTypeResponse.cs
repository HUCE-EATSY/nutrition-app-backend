using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class MealTypeResponse
{
    [JsonPropertyName("id")]
    public byte Id { get; set; }

    [JsonPropertyName("name_vi")]
    public string NameVi { get; set; } = null!;
}
