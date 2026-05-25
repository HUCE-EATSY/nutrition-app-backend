using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Foods;

public class FoodListRequest
{
    public byte? CategoryId { get; set; }

    public string? Cursor { get; set; }

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
