using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace nutrition_app_backend.DTOs.Foods;

/// <summary>
/// Request body for POST /api/foods/estimate-nutrients.
/// </summary>
public class EstimateNutrientsRequest
{
    /// <summary>
    /// Image file to upload and analyze.
    /// </summary>
    [Required(ErrorMessage = "Image là bắt buộc.")]
    public IFormFile Image { get; set; } = null!;
}
