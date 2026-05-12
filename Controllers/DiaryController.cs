using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using System.Security.Claims;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Tạm thời comment để test
public class DiaryController : ControllerBase
{
    // Tạm thời return mock data để frontend hoạt động
    // TODO: Implement real database operations

    [HttpGet]
    public ActionResult<ApiResponse<object>> GetDiary([FromQuery] string date)
    {
        // Mock data structure
        var mockData = new
        {
            date = date,
            targetCalories = 2000,
            targetProteinGram = 120,
            targetCarbGram = 250,
            targetFatGram = 67,
            consumedCalories = 0,
            consumedProteinGram = 0,
            consumedCarbGram = 0,
            consumedFatGram = 0,
            slots = new List<object>()
        };

        return Ok(ApiResponse<object>.Success(mockData));
    }

    [HttpGet("exercises")]
    public ActionResult<ApiResponse<List<object>>> GetExercises([FromQuery] string date)
    {
        return Ok(ApiResponse<List<object>>.Success(new List<object>()));
    }

    [HttpPost("entries")]
    public ActionResult<ApiResponse<object>> CreateEntry([FromBody] CreateFoodLogDto dto)
    {
        // Tạm thời chỉ return success
        // TODO: Save to database
        
        var result = new
        {
            id = Guid.NewGuid(),
            foodId = dto.FoodId,
            dateISO = dto.DateISO,
            hour = dto.Hour,
            quantityG = dto.QuantityG,
            totalCalories = dto.TotalCalories,
            proteinGram = dto.ProteinGram,
            carbGram = dto.CarbGram,
            fatGram = dto.FatGram,
            createdAt = DateTime.UtcNow
        };

        return Ok(ApiResponse<object>.Success(result, "Đã lưu bữa ăn thành công"));
    }

    [HttpPost("exercises")]
    public ActionResult<ApiResponse<object>> CreateExercise([FromBody] object dto)
    {
        return Ok(ApiResponse<object>.Success(null, "Đã lưu hoạt động thành công"));
    }

    [HttpDelete("entries/{id}")]
    public ActionResult<ApiResponse<object>> DeleteEntry(string id)
    {
        return Ok(ApiResponse<object>.Success(null, "Đã xóa thành công"));
    }
}

// DTOs
public class CreateFoodLogDto
{
    public int FoodId { get; set; }
    public string DateISO { get; set; } = string.Empty;
    public int Hour { get; set; }
    public decimal QuantityG { get; set; }
    public decimal TotalCalories { get; set; }
    public decimal ProteinGram { get; set; }
    public decimal CarbGram { get; set; }
    public decimal FatGram { get; set; }
}
