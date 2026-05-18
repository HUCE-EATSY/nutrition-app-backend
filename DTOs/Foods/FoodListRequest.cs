using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Foods;

public class FoodListRequest
{
    public byte? CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 20;
}
