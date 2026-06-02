using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Services.Streak;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace nutrition_app_backend.Controllers
{
    [ApiController]
    [Route("api/streaks")]
    [Route("api/streak")]
    [Authorize]
    public class StreakController : ControllerBase
    {
        private readonly WaoDbContext _context;
        private readonly IStreakService _streakService;

        public StreakController(WaoDbContext context, IStreakService streakService)
        {
            _context = context;
            _streakService = streakService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<object>>> GetMyStreak()
        {
            Guid userId = User.GetUserId();

            var streak = await _context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
            if (streak == null)
            {
                streak = new UserStreak { UserId = userId };
                _context.UserStreaks.Add(streak);
                await _context.SaveChangesAsync();
            }

            // Use Vietnam timezone (UTC+7) for correct day boundary calculation
            var vietnamTz = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
            var todayVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTz).Date;

            var weeklyProgress = new bool[7];
            var dayOfWeek = (int)todayVn.DayOfWeek;
            var indexToday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
            var startOfWeekVn = todayVn.AddDays(-indexToday);

            // Rule: Streak chỉ được tính khi log đủ ngưỡng 50% BMR
            var activeGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            decimal bmrThreshold = (activeGoal?.BmrKcal ?? 1600m) * 0.5m;

            var dailyCalories = await _context.FoodLogs
                .Where(f => f.UserId == userId && f.LogDate >= startOfWeekVn && f.LogDate < todayVn.AddDays(1))
                .GroupBy(f => f.LogDate.Date)
                .Select(g => new { Date = g.Key, TotalCalories = g.Sum(f => f.CaloriesKcal) })
                .ToListAsync();

            foreach (var item in dailyCalories)
            {
                if (item.TotalCalories >= bmrThreshold)
                {
                    var diff = (item.Date - startOfWeekVn).Days;
                    if (diff >= 0 && diff < 7)
                        weeklyProgress[diff] = true;
                }
            }

            // Mark freeze days as "completed" so UI dot shows green (shield icon)
            var freezes = await _context.StreakFreezeTransactions
                .Where(f => f.UserId == userId && f.FreezeDate >= startOfWeekVn && f.FreezeDate <= todayVn)
                .Select(f => f.FreezeDate)
                .ToListAsync();

            foreach (var f in freezes)
            {
                var diff = (int)(f.Date - startOfWeekVn).TotalDays;
                if (diff >= 0 && diff < 7)
                    weeklyProgress[diff] = true;
            }

            // Count shields used this week
            var freezesUsedThisWeek = freezes.Count;

            var result = new
            {
                currentStreak = streak.CurrentStreak,
                longestStreak = streak.LongestStreak,
                freezeCount = streak.FreezeCount,
                freezesUsedThisWeek = freezesUsedThisWeek,
                weeklyProgress = weeklyProgress,
                isLoggedToday = streak.LastLogDate.HasValue && streak.LastLogDate.Value.AddHours(7).Date == todayVn
            };

            return Ok(ApiResponse<object>.Success(result, "Lấy thông tin streak thành công"));
        }

        [HttpPost("freeze")]
        public async Task<ActionResult<ApiResponse<object>>> UseFreezeCard()
        {
            Guid userId = User.GetUserId();

            var streak = await _context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
            if (streak == null || streak.FreezeCount <= 0)
                return BadRequest(ApiResponse<object>.Fail("Bạn không còn thẻ đóng băng nào"));

            // Use Vietnam timezone
            var vietnamTz = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
            var todayVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTz).Date;
            var yesterdayVn = todayVn.AddDays(-1);

            // Check if already frozen for yesterday
            var alreadyFrozen = await _context.StreakFreezeTransactions
                .AnyAsync(f => f.UserId == userId && f.FreezeDate.Date == yesterdayVn);

            if (alreadyFrozen)
                return BadRequest(ApiResponse<object>.Fail("Bạn đã dùng thẻ đóng băng cho hôm qua rồi"));

            // Check if yesterday user met the 50% BMR threshold (if yes, no need to waste freeze)
            var activeGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);
            decimal bmrThreshold = (activeGoal?.BmrKcal ?? 1600m) * 0.5m;

            var yesterdayCalories = await _context.FoodLogs
                .Where(f => f.UserId == userId && f.LogDate >= yesterdayVn && f.LogDate < todayVn)
                .SumAsync(f => (decimal?)f.CaloriesKcal) ?? 0m;

            bool yesterdaySuccessful = yesterdayCalories >= bmrThreshold;

            if (yesterdaySuccessful)
                return BadRequest(ApiResponse<object>.Fail("Hôm qua bạn đã ghi ăn đủ calo để giữ streak rồi, không cần dùng thẻ đóng băng"));

            // Tối đa 2 khiên/tuần
            var startOfWeekVn = todayVn.AddDays(-(todayVn.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)todayVn.DayOfWeek - 1));
            var freezesThisWeek = await _context.StreakFreezeTransactions
                .CountAsync(f => f.UserId == userId && f.FreezeDate >= startOfWeekVn && f.FreezeDate <= todayVn);

            if (freezesThisWeek >= 2)
                return BadRequest(ApiResponse<object>.Fail("Bạn đã dùng tối đa 2 khiên trong tuần này"));

            streak.FreezeCount -= 1;

            var transaction = new StreakFreezeTransaction
            {
                UserId = userId,
                FreezeDate = yesterdayVn,
                Source = 2 // 2 = Manual
            };

            _context.StreakFreezeTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(
                new { freezeCount = streak.FreezeCount, freezesUsedThisWeek = freezesThisWeek + 1 },
                "Đã sử dụng thẻ đóng băng. Chuỗi của bạn được bảo vệ!"));
        }

        [HttpGet("leaderboard")]
        public async Task<ActionResult<ApiResponse<object>>> GetLeaderboard([FromQuery] int top = 50)
        {
            var leaderboard = await _streakService.GetLeaderboardAsync(top);
            return Ok(ApiResponse<object>.Success(leaderboard, "Lấy bảng xếp hạng thành công"));
        }

        public class AdjustStreakRequest
        {
            public int StreakToAdd { get; set; }
            public int FreezeToAdd { get; set; }
        }

        [HttpPost("test/adjust")]
        public async Task<ActionResult<ApiResponse<object>>> AdjustStreak([FromBody] AdjustStreakRequest request)
        {
            Guid userId = User.GetUserId();
            await _streakService.AdjustStreakForTestAsync(userId, request.StreakToAdd, request.FreezeToAdd);
            return Ok(ApiResponse<object>.Success(new object(), "Đã cập nhật Streak thành công (Test Mode)."));
        }

        [HttpPost("sim-log")]
        public async Task<ActionResult<ApiResponse<object>>> SimulateLog()
        {
            Guid userId = User.GetUserId();

            FoodItem? foodItem = await _context.FoodItems.FirstOrDefaultAsync();
            if (foodItem == null)
            {
                foodItem = new FoodItem
                {
                    Id = Guid.NewGuid(),
                    NameVi = "Món ăn nâng chuỗi tự động",
                    NameEn = "Auto Streak Booster",
                    CategoryId = 1,
                    Status = (nutrition_app_backend.Enums.FoodStatus)1,
                    ServingSizeG = 100,
                    Nutrition = new FoodNutrition
                    {
                        CaloriesKcal = 500,
                        ProteinG = 20,
                        CarbsG = 50,
                        FatG = 10
                    }
                };
                _context.FoodItems.Add(foodItem);
                await _context.SaveChangesAsync();
            }

            UserStreak? streak = await _context.UserStreaks.Include(s => s.User).ThenInclude(u => u.Goals).FirstOrDefaultAsync(s => s.UserId == userId);
            if (streak == null)
            {
                streak = new UserStreak { UserId = userId };
                _context.UserStreaks.Add(streak);
                await _context.SaveChangesAsync();
            }

            if (streak.FreezeCount < 3)
            {
                streak.FreezeCount = 3;
            }

            UserGoal? activeGoal = streak.User.Goals.Where(g => g.IsActive).OrderByDescending(g => g.CreatedAt).FirstOrDefault() 
                                 ?? streak.User.Goals.OrderByDescending(g => g.CreatedAt).FirstOrDefault();
            decimal bmr = activeGoal?.BmrKcal ?? 1500m;
            decimal targetKcal = bmr * 0.5m;

            DateTime yesterday = DateTime.UtcNow.AddHours(7).Date.AddDays(-1);
            
            List<FoodLog> existingLogs = await _context.FoodLogs
                .Where(f => f.UserId == userId && f.LogDate >= yesterday && f.LogDate < yesterday.AddDays(1))
                .ToListAsync();
            _context.FoodLogs.RemoveRange(existingLogs);

            FoodLog log = new FoodLog
            {
                UserId = userId,
                FoodItemId = foodItem.Id,
                MealTypeId = 1, // Breakfast
                LogDate = yesterday,
                QuantityG = (targetKcal / 500m) * 100m + 10m,
                CaloriesKcal = targetKcal + 100m,
                ProteinG = 30m,
                CarbsG = 100m,
                FatG = 15m
            };

            _context.FoodLogs.Add(log);

            // Instant Evaluation
            if (log.CaloriesKcal >= targetKcal)
            {
                if (!streak.LastLogDate.HasValue || streak.LastLogDate.Value.AddHours(7).Date < DateTime.UtcNow.AddHours(7).Date)
                {
                    streak.CurrentStreak += 1;
                    if (streak.CurrentStreak > streak.LongestStreak)
                    {
                        streak.LongestStreak = streak.CurrentStreak;
                    }
                    streak.LastLogDate = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new { currentStreak = streak.CurrentStreak }, "Cập nhật chuỗi thành công!"));
        }
    }
}
