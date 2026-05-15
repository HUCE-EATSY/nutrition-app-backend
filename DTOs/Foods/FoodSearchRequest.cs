using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Foods;

public class FoodSearchRequest
{
    [Required(ErrorMessage = "Từ khóa tìm kiếm không được để trống.")]
    [MinLength(1, ErrorMessage = "Từ khóa tìm kiếm không được để trống.")]
    public string Q { get; set; } = null!;

    public byte? CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 20;
}
