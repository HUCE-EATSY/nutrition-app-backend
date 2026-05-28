using System.Text.Json;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.DTOs.Spoonacular;
using nutrition_app_backend.Exceptions;

namespace nutrition_app_backend.Services.Spoonacular;

public class SpoonacularService : ISpoonacularService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<SpoonacularService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SpoonacularService(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<SpoonacularService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Spoonacular");
        _config     = config;
        _logger     = logger;
    }

    /// <inheritdoc/>
    public async Task<EstimatedFoodResponse?> EstimateNutrientsAsync(string imageUrl)
    {
        // STEP 1 — Transform .webp → .jpg so Cloudinary delivers a compatible format
        var jpgUrl = TransformToJpg(imageUrl);

        var apiKey = _config["Spoonacular:ApiKey"]
            ?? throw new InvalidOperationException("Spoonacular:ApiKey chưa được cấu hình.");

        // STEP 2 — Call Spoonacular food/images/analyze
        SpoonacularEstimateDto? dto;
        try
        {
            var url      = $"food/images/analyze?imageUrl={Uri.EscapeDataString(jpgUrl)}&apiKey={apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Spoonacular trả về {StatusCode} cho ảnh {ImageUrl}",
                    (int)response.StatusCode, jpgUrl);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            dto = JsonSerializer.Deserialize<SpoonacularEstimateDto>(content, _jsonOpts);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout khi gọi Spoonacular với ảnh {ImageUrl}", jpgUrl);
            throw new BusinessException("SPOONACULAR_ERROR",
                "Yêu cầu tới Spoonacular bị timeout. Vui lòng thử lại.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Lỗi kết nối khi gọi Spoonacular với ảnh {ImageUrl}", jpgUrl);
            throw new BusinessException("SPOONACULAR_ERROR",
                "Không thể kết nối đến dịch vụ phân tích dinh dưỡng. Vui lòng thử lại sau.");
        }

        if (dto == null)
        {
            _logger.LogWarning("Spoonacular trả về response rỗng cho ảnh {ImageUrl}", jpgUrl);
            return null;
        }

        // STEP 3 — Map Spoonacular category and nutrition object to CreateFoodNutritionDto
        var nutrition = dto.Nutrition;
        var calories = nutrition?.Calories?.Value ?? 0m;
        var protein = nutrition?.Protein?.Value ?? 0m;
        var carbs = nutrition?.Carbs?.Value ?? 0m;
        var fat = nutrition?.Fat?.Value ?? 0m;

        return new EstimatedFoodResponse
        {
            NameEn      = dto.Category?.Name ?? "Unknown",
            ImageUrl    = jpgUrl,
            ServingSizeG = 100m,
            Nutrition   = new CreateFoodNutritionDto
            {
                CaloriesKcal = calories,
                ProteinG     = protein,
                CarbsG       = carbs,
                FatG         = fat,
                FiberG       = null,
                SugarG       = null,
                SodiumMg     = null
            }
        };
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    /// <summary>
    /// Replaces the file extension of a Cloudinary URL from .webp to .jpg.
    /// If the URL does not end with .webp, returns it unchanged so the method
    /// is safe to call with any Cloudinary URL.
    /// </summary>
    private static string TransformToJpg(string url)
    {
        if (url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            return url[..^5] + ".jpg";

        return url; // Already .jpg or another format — pass through
    }
}
