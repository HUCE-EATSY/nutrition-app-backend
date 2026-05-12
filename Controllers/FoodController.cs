using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Services.Foods;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FoodController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodController(IFoodService foodService)
    {
        _foodService = foodService;
    }

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
    }
}
