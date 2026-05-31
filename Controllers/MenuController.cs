using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Food;

namespace nutrition_app_backend.Controllers;

[Authorize]
[ApiController]
[Route("api/menus")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserMenus()
    {
        var userId = User.GetUserId();
        var menus = await _menuService.GetUserMenusAsync(userId);
        return Ok(menus);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuById(Guid id)
    {
        var userId = User.GetUserId();
        var menu = await _menuService.GetMenuByIdAsync(id, userId);
        if (menu == null)
            return NotFound(new { message = "Không tìm thấy thực đơn." });

        return Ok(menu);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.GetUserId();
        var menu = await _menuService.CreateMenuAsync(userId, request);
        return CreatedAtAction(nameof(GetMenuById), new { id = menu.Id }, menu);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] UpdateMenuRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.GetUserId();
        var menu = await _menuService.UpdateMenuAsync(id, userId, request);
        return Ok(menu);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenu(Guid id)
    {
        var userId = User.GetUserId();
        await _menuService.DeleteMenuAsync(id, userId);
        return NoContent();
    }

    [HttpPost("{id}/foods")]
    public async Task<IActionResult> AddFoodToMenu(Guid id, [FromBody] AddFoodToMenuRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.GetUserId();
        var menu = await _menuService.AddFoodToMenuAsync(id, userId, request);
        return Ok(menu);
    }

    [HttpDelete("{id}/foods/{foodId}")]
    public async Task<IActionResult> RemoveFoodFromMenu(Guid id, Guid foodId)
    {
        var userId = User.GetUserId();
        var menu = await _menuService.RemoveFoodFromMenuAsync(id, foodId, userId);
        return Ok(menu);
    }
}
