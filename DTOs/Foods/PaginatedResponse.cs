using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class PaginatedResponse<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
