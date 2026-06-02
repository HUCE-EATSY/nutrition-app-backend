using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Diaries;

namespace nutrition_app_backend.Controllers
{
    [ApiController]
    [Route("api/menus")]
    [Authorize]
    public class MenusController : ControllerBase
    {
        private readonly WaoDbContext _context;

        public MenusController(WaoDbContext context)
        {
            _context = context;
        }

        // ==============================
        // GET /api/menus/my-plans
        // ==============================
        [HttpGet("my-plans")]
        public async Task<ActionResult<ApiResponse<List<MenuResponse>>>> GetMyPlans()
        {
            Guid userId = User.GetUserId();

            List<Menu> menus = await _context.Menus
                .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            List<MenuResponse> response = new List<MenuResponse>();
            foreach (Menu menu in menus)
            {
                response.Add(MapToMenuResponse(menu));
            }

            return Ok(ApiResponse<List<MenuResponse>>.Success(response, "Lấy danh sách thực đơn thành công"));
        }

        // ==============================
        // POST /api/menus
        // ==============================
        [HttpPost]
        public async Task<ActionResult<ApiResponse<MenuResponse>>> CreateMenu([FromBody] CreateMenuRequest request)
        {
            Guid userId = User.GetUserId();

            // Calculate overall macros first
            decimal totalCalories = 0m;
            decimal totalProtein = 0m;
            decimal totalCarbs = 0m;
            decimal totalFat = 0m;

            List<MenuFood> menuFoods = new List<MenuFood>();
            Guid menuId = Guid.NewGuid();

            foreach (CreateMenuFoodRequest item in request.Foods)
            {
                var food = await _context.FoodItems
                    .Include(f => f.Nutrition)
                    .FirstOrDefaultAsync(f => f.Id == item.FoodItemId);

                if (food == null)
                {
                    return BadRequest(ApiResponse<MenuResponse>.Fail("Không tìm thấy món ăn trong hệ thống"));
                }

                decimal ratio = food.ServingSizeG > 0 ? item.QuantityG / food.ServingSizeG : 1m;
                if (food.Nutrition != null)
                {
                    totalCalories += food.Nutrition.CaloriesKcal * ratio;
                    totalProtein += food.Nutrition.ProteinG * ratio;
                    totalCarbs += food.Nutrition.CarbsG * ratio;
                    totalFat += food.Nutrition.FatG * ratio;
                }

                menuFoods.Add(new MenuFood
                {
                    Id = Guid.NewGuid(),
                    MenuId = menuId,
                    FoodItemId = item.FoodItemId,
                    MealTypeId = item.MealTypeId,
                    QuantityG = item.QuantityG
                });
            }

            Menu menu = new Menu
            {
                Id = menuId,
                Name = request.Name,
                Description = request.Description,
                CoverImageUrl = request.CoverImageUrl,
                UserId = userId,
                TotalCalories = totalCalories,
                TotalProtein = totalProtein,
                TotalCarbs = totalCarbs,
                TotalFat = totalFat,
                CreatedAt = DateTime.UtcNow,
                MenuFoods = menuFoods
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            // Reload menu to return with full includes
            Menu? createdMenu = await _context.Menus
                .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .FirstOrDefaultAsync(m => m.Id == menuId);

            if (createdMenu == null)
            {
                return StatusCode(500, ApiResponse<MenuResponse>.Fail("Lỗi hệ thống khi tạo thực đơn"));
            }

            return StatusCode(201, ApiResponse<MenuResponse>.Success(MapToMenuResponse(createdMenu), "Tạo thực đơn cá nhân thành công"));
        }

        // ==============================
        // PUT /api/menus/{id}
        // ==============================
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<MenuResponse>>> UpdateMenu(Guid id, [FromBody] CreateMenuRequest request)
        {
            Guid userId = User.GetUserId();

            Menu? menu = await _context.Menus
                .Include(m => m.MenuFoods)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (menu == null)
            {
                return NotFound(ApiResponse<MenuResponse>.Fail("Không tìm thấy thực đơn hoặc bạn không có quyền chỉnh sửa"));
            }

            // Remove existing food items
            _context.MenuFoods.RemoveRange(menu.MenuFoods);

            decimal totalCalories = 0m;
            decimal totalProtein = 0m;
            decimal totalCarbs = 0m;
            decimal totalFat = 0m;

            List<MenuFood> menuFoods = new List<MenuFood>();

            foreach (CreateMenuFoodRequest item in request.Foods)
            {
                var food = await _context.FoodItems
                    .Include(f => f.Nutrition)
                    .FirstOrDefaultAsync(f => f.Id == item.FoodItemId);

                if (food == null)
                {
                    return BadRequest(ApiResponse<MenuResponse>.Fail("Không tìm thấy món ăn trong hệ thống"));
                }

                decimal ratio = food.ServingSizeG > 0 ? item.QuantityG / food.ServingSizeG : 1m;
                if (food.Nutrition != null)
                {
                    totalCalories += food.Nutrition.CaloriesKcal * ratio;
                    totalProtein += food.Nutrition.ProteinG * ratio;
                    totalCarbs += food.Nutrition.CarbsG * ratio;
                    totalFat += food.Nutrition.FatG * ratio;
                }

                menuFoods.Add(new MenuFood
                {
                    Id = Guid.NewGuid(),
                    MenuId = id,
                    FoodItemId = item.FoodItemId,
                    MealTypeId = item.MealTypeId,
                    QuantityG = item.QuantityG
                });
            }

            menu.Name = request.Name;
            menu.Description = request.Description;
            menu.CoverImageUrl = request.CoverImageUrl;
            menu.TotalCalories = totalCalories;
            menu.TotalProtein = totalProtein;
            menu.TotalCarbs = totalCarbs;
            menu.TotalFat = totalFat;
            menu.MenuFoods = menuFoods;

            await _context.SaveChangesAsync();

            // Reload menu
            Menu? updatedMenu = await _context.Menus
                .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (updatedMenu == null)
            {
                return StatusCode(500, ApiResponse<MenuResponse>.Fail("Lỗi hệ thống khi chỉnh sửa thực đơn"));
            }

            return Ok(ApiResponse<MenuResponse>.Success(MapToMenuResponse(updatedMenu), "Cập nhật thực đơn thành công"));
        }

        // ==============================
        // DELETE /api/menus/{id}
        // ==============================
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMenu(Guid id)
        {
            Guid userId = User.GetUserId();

            Menu? menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (menu == null)
            {
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy thực đơn hoặc bạn không có quyền xóa"));
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new object(), "Xóa thực đơn thành công"));
        }

        // ==============================
        // Helper mapping
        // ==============================
        private MenuResponse MapToMenuResponse(Menu menu)
        {
            MenuResponse response = new MenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Description = menu.Description,
                CoverImageUrl = menu.CoverImageUrl,
                UserId = menu.UserId,
                TotalCalories = menu.TotalCalories,
                TotalProtein = menu.TotalProtein,
                TotalCarbs = menu.TotalCarbs,
                TotalFat = menu.TotalFat,
                CreatedAt = menu.CreatedAt,
                Foods = new List<MenuFoodResponse>()
            };

            foreach (MenuFood mf in menu.MenuFoods)
            {
                decimal ratio = mf.FoodItem.ServingSizeG > 0 ? mf.QuantityG / mf.FoodItem.ServingSizeG : 1m;
                response.Foods.Add(new MenuFoodResponse
                {
                    FoodItemId = mf.FoodItemId,
                    NameVi = mf.FoodItem.NameVi,
                    NameEn = mf.FoodItem.NameEn ?? "Food",
                    ImageUrl = mf.FoodItem.ActiveImage?.StoragePath,
                    MealTypeId = mf.MealTypeId,
                    QuantityG = mf.QuantityG,
                    CaloriesKcal = (mf.FoodItem.Nutrition?.CaloriesKcal ?? 0m) * ratio,
                    ProteinG = (mf.FoodItem.Nutrition?.ProteinG ?? 0m) * ratio,
                    CarbsG = (mf.FoodItem.Nutrition?.CarbsG ?? 0m) * ratio,
                    FatG = (mf.FoodItem.Nutrition?.FatG ?? 0m) * ratio
                });
            }

            return response;
        }
    }
}
