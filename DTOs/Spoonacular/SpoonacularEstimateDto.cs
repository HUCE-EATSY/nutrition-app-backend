using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Spoonacular;

/// <summary>
/// Internal deserialization target for Spoonacular GET /food/images/analyze.
/// NOT exposed as a public API response.
/// </summary>
public class SpoonacularEstimateDto
{
    [JsonPropertyName("category")]
    public SpoonacularCategoryDto? Category { get; set; }

    [JsonPropertyName("nutrition")]
    public SpoonacularNutritionDto? Nutrition { get; set; }
}

public class SpoonacularCategoryDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("probability")]
    public double Probability { get; set; }
}

public class SpoonacularNutritionDto
{
    [JsonPropertyName("calories")]
    public SpoonacularNutrientValueDto? Calories { get; set; }

    [JsonPropertyName("fat")]
    public SpoonacularNutrientValueDto? Fat { get; set; }

    [JsonPropertyName("protein")]
    public SpoonacularNutrientValueDto? Protein { get; set; }

    [JsonPropertyName("carbs")]
    public SpoonacularNutrientValueDto? Carbs { get; set; }
}

public class SpoonacularNutrientValueDto
{
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}
