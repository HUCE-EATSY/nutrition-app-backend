using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Services.Admin;

namespace nutrition_app_backend.Controllers.Admin;

[ApiController]
[Route("api/admin/foods")]
public class AdminFoodsController : ControllerBase
{
    private readonly IAdminFoodService _foodService;

    public AdminFoodsController(IAdminFoodService foodService)
    {
        _foodService = foodService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFoods([FromQuery] int page = 1, [FromQuery] string? search = null, [FromQuery] byte? categoryId = null)
    {
        var foods = await _foodService.GetAllFoodsAsync(page, 20, search, categoryId);
        return Ok(new { success = true, data = foods });
    }

    [HttpPost]
    public async Task<IActionResult> CreateFood([FromBody] AdminFoodCreateDto dto)
    {
        // Mock Admin ID for now since we haven't implemented Auth
        var adminId = Guid.Empty; 
        var food = await _foodService.CreateFoodAsync(dto, adminId);
        return Ok(new { success = true, data = food });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFood(Guid id, [FromBody] AdminFoodUpdateDto dto)
    {
        var food = await _foodService.UpdateFoodAsync(id, dto);
        return Ok(new { success = true, data = food });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFood(Guid id)
    {
        await _foodService.DeleteFoodAsync(id);
        return Ok(new { success = true });
    }

    [HttpPut("{id}/toggle-visibility")]
    public async Task<IActionResult> ToggleVisibility(Guid id)
    {
        await _foodService.ToggleVisibilityAsync(id);
        return Ok(new { success = true });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _foodService.GetStatsAsync();
        return Ok(new { success = true, data = stats });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _foodService.GetCategoriesAsync();
        return Ok(new { success = true, data = categories });
    }
}
