using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.OpenFoodFacts;

/// <summary>
/// Internal DTO — chỉ dùng để deserialize JSON từ Open Food Facts API.
/// Không expose ra ngoài mobile client.
/// </summary>
public class OffProductDto
{
    /// <summary>1 = tìm thấy, 0 = không tìm thấy</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("product")]
    public OffProduct? Product { get; set; }
}

public class OffProduct
{
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("product_name_en")]
    public string? ProductNameEn { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("nutriments")]
    public OffNutriments? Nutriments { get; set; }
}

public class OffNutriments
{
    /// <summary>Calories tính trên 100g — đơn vị Kcal</summary>
    [JsonPropertyName("energy-kcal_100g")]
    public double? EnergyKcal100g { get; set; }

    [JsonPropertyName("proteins_100g")]
    public double? Proteins100g { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public double? Carbs100g { get; set; }

    [JsonPropertyName("fat_100g")]
    public double? Fat100g { get; set; }

    [JsonPropertyName("fiber_100g")]
    public double? Fiber100g { get; set; }

    [JsonPropertyName("sugars_100g")]
    public double? Sugars100g { get; set; }

    /// <summary>OFF trả về g/100g — phải nhân 1000 khi map sang SodiumMg</summary>
    [JsonPropertyName("sodium_100g")]
    public double? Sodium100g { get; set; }
}
