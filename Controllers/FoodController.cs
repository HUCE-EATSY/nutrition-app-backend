<<<<<<< HEAD
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Food;
=======
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Services.Foods;
>>>>>>> feature/phase-2-food-db-and-logging

namespace nutrition_app_backend.Controllers;

[ApiController]
<<<<<<< HEAD
[Route("api/[controller]")]
[Authorize]
=======
[Route("api/v1/[controller]")]
>>>>>>> feature/phase-2-food-db-and-logging
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

<<<<<<< HEAD
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
=======
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<FoodDto>>>> GetAll([FromQuery] string? category, [FromQuery] string? search)
    {
        var foods = await _foodService.GetAllAsync(category, search);
        return Ok(ApiResponse<IEnumerable<FoodDto>>.Success(foods));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<FoodDto>>> GetById(int id)
    {
        var food = await _foodService.GetByIdAsync(id);
        
        if (food == null)
            return NotFound(ApiResponse<FoodDto>.Fail("Food not found", "404"));

        return Ok(ApiResponse<FoodDto>.Success(food));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<FoodDto>>> Create([FromBody] CreateFoodDto dto)
    {
        var food = await _foodService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = food.Id }, ApiResponse<FoodDto>.Success(food, "Food created successfully", "201"));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<FoodDto>>> Update(int id, [FromBody] UpdateFoodDto dto)
    {
        var food = await _foodService.UpdateAsync(id, dto);
        
        if (food == null)
            return NotFound(ApiResponse<FoodDto>.Fail("Food not found", "404"));

        return Ok(ApiResponse<FoodDto>.Success(food, "Food updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var success = await _foodService.DeleteAsync(id);
        
        if (!success)
            return NotFound(ApiResponse<object>.Fail("Food not found", "404"));

        return Ok(ApiResponse<object>.Success(null!, "Food deleted successfully"));
>>>>>>> feature/phase-2-food-db-and-logging
    }
}
