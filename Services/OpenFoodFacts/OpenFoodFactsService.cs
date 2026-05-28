using System.Text.Json;
using nutrition_app_backend.DTOs.OpenFoodFacts;

namespace nutrition_app_backend.Services.OpenFoodFacts;

public class OpenFoodFactsService : IOpenFoodFactsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenFoodFactsService> _logger;

    public OpenFoodFactsService(IHttpClientFactory httpClientFactory,
                                ILogger<OpenFoodFactsService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("OpenFoodFacts");
        _logger     = logger;
    }

    /// <inheritdoc/>
    public async Task<OffProductDto?> LookupByBarcodeAsync(string barcode)
    {
        try
        {
            // Chỉ lấy các fields cần thiết để giảm payload
            var url      = $"api/v2/product/{barcode}.json?fields=product_name,product_name_en,image_url,nutriments";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OFF API trả về {StatusCode} cho mã vạch {Barcode}",
                    response.StatusCode, barcode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var dto     = JsonSerializer.Deserialize<OffProductDto>(content);

            // OFF trả về status=0 khi không tìm thấy sản phẩm
            if (dto?.Status != 1 || dto.Product == null)
            {
                _logger.LogInformation("Open Food Facts không có dữ liệu cho mã vạch {Barcode}", barcode);
                return null;
            }

            return dto;
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            _logger.LogError(ex, "Lỗi kết nối hoặc timeout khi tra cứu mã vạch {Barcode}", barcode);
            return null;
        }
    }
}
