using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Services.Food;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/meal-types")]
[Authorize]
public class MealTypesController : ControllerBase
{
    private readonly IFoodService _foodService;

    public MealTypesController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    /// <summary>
    /// Get all meal types (sáng/trưa/tối/phụ).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MealTypeResponse>>>> GetAll()
    {
        var result = await _foodService.GetMealTypesAsync();

        return Ok(ApiResponse<List<MealTypeResponse>>.Success(result, "Lấy danh sách bữa ăn thành công"));
    }
}
