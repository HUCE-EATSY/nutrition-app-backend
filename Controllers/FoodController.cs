using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Food;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    /// <summary>
    /// Tìm kiếm món ăn theo tên.
    /// GET /api/food?search=chuoi
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search)
    {
        var results = await _foodService.SearchFoodsAsync(search);
        return Ok(ApiResponse<List<FoodResponse>>.Success(results, "Tìm kiếm thành công"));
    }

    /// <summary>
    /// Lấy chi tiết một món ăn.
    /// GET /api/food/{id}
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var food = await _foodService.GetFoodByIdAsync(id);
        if (food == null)
            return NotFound(ApiResponse<object>.Fail("Không tìm thấy món ăn", "404"));

        return Ok(ApiResponse<FoodResponse>.Success(food, "Lấy thông tin thành công"));
    }

    /// <summary>
    /// Tạo món ăn mới (Giai đoạn 1: không upload ảnh).
    /// POST /api/food
    /// Body: { name, caloriesPer100g, proteinPer100g, carbPer100g, fatPer100g, imageUrl? }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFoodRequest request)
    {
        Guid userId = User.GetUserId();
        var food = await _foodService.CreateFoodAsync(userId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = food.Id },
            ApiResponse<FoodResponse>.Success(food, "Tạo món ăn thành công")
        );
    }
}
