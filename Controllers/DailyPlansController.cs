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
    [Route("api/daily-plans")]
    [Authorize]
    public class DailyPlansController : ControllerBase
    {
        private readonly WaoDbContext _context;

        public DailyPlansController(WaoDbContext context)
        {
            _context = context;
        }

        // ==============================
        // POST /api/daily-plans/apply
        // ==============================
        [HttpPost("apply")]
        public async Task<ActionResult<ApiResponse<object>>> ApplyDailyPlan([FromBody] ApplyDailyPlanRequest request)
        {
            Guid userId = User.GetUserId();

            Menu? menu = await _context.Menus
                .Include(m => m.MenuFoods)
                .ThenInclude(mf => mf.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .FirstOrDefaultAsync(m => m.Id == request.MenuId && m.UserId == userId);

            if (menu == null)
            {
                return NotFound(ApiResponse<object>.Fail("Không tìm thấy thực đơn hoặc bạn không có quyền áp dụng"));
            }

            DateTime logDate = DateTime.UtcNow.Date;
            if (!string.IsNullOrEmpty(request.Date) && DateTime.TryParse(request.Date, out DateTime parsedDate))
            {
                logDate = parsedDate.Date;
            }

            // Remove any existing planned items for this date to avoid duplicates
            List<DailyPlan> existingPlans = await _context.DailyPlans
                .Where(p => p.UserId == userId && p.LogDate == logDate)
                .ToListAsync();

            _context.DailyPlans.RemoveRange(existingPlans);

            foreach (MenuFood mf in menu.MenuFoods)
            {
                DailyPlan plan = new DailyPlan
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LogDate = logDate,
                    FoodItemId = mf.FoodItemId,
                    MealTypeId = mf.MealTypeId,
                    QuantityG = mf.QuantityG,
                    IsSynced = false
                };

                _context.DailyPlans.Add(plan);
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new object(), $"Đã lên lịch thực đơn '{menu.Name}' thành công cho ngày {logDate:yyyy-MM-dd}"));
        }

        // ==============================
        // GET /api/daily-plans/{date}
        // ==============================
        [HttpGet("{date}")]
        public async Task<ActionResult<ApiResponse<DailyPlanResponse>>> GetDailyPlan([FromRoute] string date)
        {
            Guid userId = User.GetUserId();

            DateTime logDate = DateTime.UtcNow.Date;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime parsedDate))
            {
                logDate = parsedDate.Date;
            }

            List<DailyPlan> plans = await _context.DailyPlans
                .AsNoTracking()
                .Include(p => p.FoodItem)
                .ThenInclude(fi => fi.ActiveImage)
                .Include(p => p.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .Where(p => p.UserId == userId && p.LogDate == logDate)
                .ToListAsync();

            DailyPlanResponse response = new DailyPlanResponse
            {
                LogDate = logDate,
                Items = new List<DailyPlanItemResponse>()
            };

            foreach (DailyPlan plan in plans)
            {
                decimal ratio = plan.FoodItem.ServingSizeG > 0 ? plan.QuantityG / plan.FoodItem.ServingSizeG : 1m;
                DailyPlanItemResponse item = new DailyPlanItemResponse
                {
                    Id = plan.Id,
                    FoodItemId = plan.FoodItemId,
                    FoodNameVi = plan.FoodItem.NameVi,
                    FoodNameEn = plan.FoodItem.NameEn ?? "Food",
                    ImageUrl = plan.FoodItem.ActiveImage?.StoragePath,
                    MealTypeId = plan.MealTypeId,
                    QuantityG = plan.QuantityG,
                    CaloriesKcal = (plan.FoodItem.Nutrition?.CaloriesKcal ?? 0m) * ratio,
                    ProteinG = (plan.FoodItem.Nutrition?.ProteinG ?? 0m) * ratio,
                    CarbsG = (plan.FoodItem.Nutrition?.CarbsG ?? 0m) * ratio,
                    FatG = (plan.FoodItem.Nutrition?.FatG ?? 0m) * ratio,
                    IsSynced = plan.IsSynced
                };
                response.Items.Add(item);
            }

            return Ok(ApiResponse<DailyPlanResponse>.Success(response, "Lấy lịch ăn hàng ngày thành công"));
        }

        // ==============================
        // POST /api/daily-plans/sync-to-diary
        // ==============================
        [HttpPost("sync-to-diary")]
        public async Task<ActionResult<ApiResponse<DailyPlanResponse>>> SyncToDiary([FromBody] SyncToDiaryRequest request)
        {
            Guid userId = User.GetUserId();

            DateTime logDate = DateTime.UtcNow.Date;
            if (!string.IsNullOrEmpty(request.Date) && DateTime.TryParse(request.Date, out DateTime parsedDate))
            {
                logDate = parsedDate.Date;
            }

            List<DailyPlan> plans = await _context.DailyPlans
                .Include(p => p.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .Where(p => p.UserId == userId && p.LogDate == logDate && p.MealTypeId == request.MealTypeId && !p.IsSynced)
                .ToListAsync();

            if (plans.Count == 0)
            {
                return await BuildPlanResponse(userId, logDate, "Không có món ăn nào cần ghi nhận cho bữa này.");
            }

            // Map meal type ID to standard hours
            int logHour = request.MealTypeId switch
            {
                1 => 7,   // Sáng: 07:00
                2 => 12,  // Trưa: 12:00
                3 => 19,  // Tối: 19:00
                4 => 22,  // Phụ: 22:00
                _ => 7
            };

            DateTime logDateTime = new DateTime(logDate.Year, logDate.Month, logDate.Day, logHour, 0, 0, DateTimeKind.Local).ToUniversalTime();
            List<Guid> planIds = plans.Select(p => p.Id).ToList();

            foreach (DailyPlan plan in plans)
            {
                decimal ratio = plan.FoodItem.ServingSizeG > 0 ? plan.QuantityG / plan.FoodItem.ServingSizeG : 1m;

                // Check for duplicate to avoid multiple ticks adding duplicates
                bool alreadyLogged = await _context.FoodLogs.AnyAsync(fl =>
                    fl.UserId == userId &&
                    fl.FoodItemId == plan.FoodItemId &&
                    fl.MealTypeId == plan.MealTypeId &&
                    fl.LogDate == logDate &&
                    fl.QuantityG == plan.QuantityG);

                if (!alreadyLogged)
                {
                    FoodLog log = new FoodLog
                    {
                        UserId = userId,
                        FoodItemId = plan.FoodItemId,
                        MealTypeId = plan.MealTypeId,
                        LogDate = logDate,
                        QuantityG = plan.QuantityG,
                        CaloriesKcal = (plan.FoodItem.Nutrition?.CaloriesKcal ?? 0m) * ratio,
                        ProteinG = (plan.FoodItem.Nutrition?.ProteinG ?? 0m) * ratio,
                        CarbsG = (plan.FoodItem.Nutrition?.CarbsG ?? 0m) * ratio,
                        FatG = (plan.FoodItem.Nutrition?.FatG ?? 0m) * ratio,
                        CreatedAt = logDateTime
                    };
                    _context.FoodLogs.Add(log);
                }
            }

            // Save FoodLogs
            await _context.SaveChangesAsync();

            // Direct database update for IsSynced to bypass Entity Framework state tracker and ensure immediate DB persistence
            await _context.DailyPlans
                .Where(p => planIds.Contains(p.Id))
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.IsSynced, true));

            string mealName = request.MealTypeId switch
            {
                1 => "Bữa sáng",
                2 => "Bữa trưa",
                3 => "Bữa tối",
                4 => "Bữa phụ",
                _ => "Bữa ăn"
            };

            return await BuildPlanResponse(userId, logDate, $"Đã ghi nhận {plans.Count} món ăn vào nhật ký {mealName} thành công!");
        }

        // ==============================
        // Helper: Build refreshed DailyPlanResponse
        // ==============================
        private async Task<ActionResult<ApiResponse<DailyPlanResponse>>> BuildPlanResponse(Guid userId, DateTime logDate, string message)
        {
            List<DailyPlan> allPlans = await _context.DailyPlans
                .AsNoTracking()
                .Include(p => p.FoodItem)
                .ThenInclude(fi => fi.ActiveImage)
                .Include(p => p.FoodItem)
                .ThenInclude(fi => fi.Nutrition)
                .Where(p => p.UserId == userId && p.LogDate == logDate)
                .ToListAsync();

            DailyPlanResponse response = new DailyPlanResponse
            {
                LogDate = logDate,
                Items = new List<DailyPlanItemResponse>()
            };

            foreach (DailyPlan plan in allPlans)
            {
                decimal ratio = plan.FoodItem.ServingSizeG > 0 ? plan.QuantityG / plan.FoodItem.ServingSizeG : 1m;
                response.Items.Add(new DailyPlanItemResponse
                {
                    Id = plan.Id,
                    FoodItemId = plan.FoodItemId,
                    FoodNameVi = plan.FoodItem.NameVi,
                    FoodNameEn = plan.FoodItem.NameEn ?? "Food",
                    ImageUrl = plan.FoodItem.ActiveImage?.StoragePath,
                    MealTypeId = plan.MealTypeId,
                    QuantityG = plan.QuantityG,
                    CaloriesKcal = (plan.FoodItem.Nutrition?.CaloriesKcal ?? 0m) * ratio,
                    ProteinG = (plan.FoodItem.Nutrition?.ProteinG ?? 0m) * ratio,
                    CarbsG = (plan.FoodItem.Nutrition?.CarbsG ?? 0m) * ratio,
                    FatG = (plan.FoodItem.Nutrition?.FatG ?? 0m) * ratio,
                    IsSynced = plan.IsSynced
                });
            }

            return Ok(ApiResponse<DailyPlanResponse>.Success(response, message));
        }
    }
}
