using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

public class CursorPaginatedResponse<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore => NextCursor != null;
}
