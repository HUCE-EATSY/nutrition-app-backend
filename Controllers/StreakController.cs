using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Models.Diaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nutrition_app_backend.Controllers
{
    [ApiController]
    [Route("api/streaks")]
    [Authorize]
    public class StreakController : ControllerBase
    {
        private readonly WaoDbContext _context;

        public StreakController(WaoDbContext context)
        {
            _context = context;
        }

        private class TempLogGroup
        {
            public DateTime LogDate { get; set; }
            public decimal TotalCals { get; set; }
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<object>>> GetMyStreak()
        {
            Guid userId = User.GetUserId();

            UserStreak? streak = await _context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
            if (streak == null)
            {
                streak = new UserStreak { UserId = userId };
                _context.UserStreaks.Add(streak);
                await _context.SaveChangesAsync();
            }

            TimeZoneInfo vietnamTz = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
            DateTime todayDt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTz).Date;
            
            bool[] weeklyProgress = new bool[7];
            int dayOfWeek = (int)todayDt.DayOfWeek;
            int indexToday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

            DateTime startOfWeekDt = todayDt.AddDays(-indexToday);
            DateTime endOfTodayDt = todayDt.AddDays(1);

            DateTime startOfWeekUtc = TimeZoneInfo.ConvertTimeToUtc(startOfWeekDt, vietnamTz);
            DateTime endOfTodayUtc = TimeZoneInfo.ConvertTimeToUtc(endOfTodayDt, vietnamTz);

            var rawLogs = await _context.FoodLogs
                .Where(f => f.UserId == userId && f.LogDate >= startOfWeekUtc && f.LogDate < endOfTodayUtc)
                .Select(f => new { f.LogDate, f.CaloriesKcal })
                .ToListAsync();

            List<TempLogGroup> logs = rawLogs
                .GroupBy(f => TimeZoneInfo.ConvertTimeFromUtc(f.LogDate, vietnamTz).Date)
                .Select(g => new TempLogGroup { LogDate = g.Key, TotalCals = g.Sum(x => x.CaloriesKcal) })
                .ToList();

            foreach (TempLogGroup log in logs)
            {
                int diff = (log.LogDate - startOfWeekDt).Days;
                if (diff >= 0 && diff < 7)
                {
                    if (log.TotalCals > 0)
                    {
                        weeklyProgress[diff] = true;
                    }
                }
            }
            
            List<DateTime> freezes = await _context.StreakFreezeTransactions
                .Where(f => f.UserId == userId && f.FreezeDate >= startOfWeekDt && f.FreezeDate <= todayDt)
                .Select(f => f.FreezeDate)
                .ToListAsync();
            
            foreach (DateTime f in freezes)
            {
                int diff = (int)(f.Date - startOfWeekDt).TotalDays;
                if (diff >= 0 && diff < 7)
                {
                    weeklyProgress[diff] = true;
                }
            }

            object result = new
            {
                currentStreak = streak.CurrentStreak,
                longestStreak = streak.LongestStreak,
                freezeCount = streak.FreezeCount,
                weeklyProgress = weeklyProgress,
                isLoggedToday = streak.LastLogDate.HasValue && TimeZoneInfo.ConvertTimeFromUtc(streak.LastLogDate.Value, vietnamTz).Date == todayDt
            };

            return Ok(ApiResponse<object>.Success(result, "Lấy thông tin streak thành công"));
        }

        [HttpPost("freeze")]
        public async Task<ActionResult<ApiResponse<object>>> UseFreezeCard()
        {
            Guid userId = User.GetUserId();

            UserStreak? streak = await _context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
            if (streak == null || streak.FreezeCount <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail("Không đủ thẻ đóng băng"));
            }

            DateTime yesterdayDt = DateTime.UtcNow.AddHours(7).Date.AddDays(-1);
            DateTime yesterdayEndDt = yesterdayDt.AddDays(1);

            bool alreadyFrozen = await _context.StreakFreezeTransactions
                .AnyAsync(f => f.UserId == userId && f.FreezeDate.Date == yesterdayDt);

            if (alreadyFrozen)
            {
                return BadRequest(ApiResponse<object>.Fail("Bạn đã dùng thẻ đóng băng cho hôm qua rồi"));
            }

            bool loggedYesterday = await _context.FoodLogs
                .AnyAsync(f => f.UserId == userId && f.LogDate >= yesterdayDt && f.LogDate < yesterdayEndDt);
            
            if (loggedYesterday)
            {
                return BadRequest(ApiResponse<object>.Fail("Hôm qua bạn đã hoàn thành mục tiêu, không cần đóng băng"));
            }

            streak.FreezeCount -= 1;

            StreakFreezeTransaction transaction = new StreakFreezeTransaction
            {
                UserId = userId,
                FreezeDate = yesterdayDt,
                Source = 2
            };

            _context.StreakFreezeTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new { freezeCount = streak.FreezeCount }, "Đã sử dụng thẻ đóng băng"));
        }

        [HttpGet("leaderboard")]
        public async Task<ActionResult<ApiResponse<object>>> GetLeaderboard()
        {
            object leaderboard = await _context.UserStreaks
                .Include(s => s.User)
                .ThenInclude(u => u.Profile)
                .OrderByDescending(s => s.CurrentStreak)
                .ThenByDescending(s => s.LongestStreak)
                .Take(50)
                .Select(s => new
                {
                    userId = s.UserId,
                    displayName = s.User.Profile != null ? s.User.Profile.DisplayName : "Người dùng WAO",
                    avatarUrl = s.User.Profile != null ? s.User.Profile.AvatarUrl : null,
                    currentStreak = s.CurrentStreak,
                    longestStreak = s.LongestStreak
                })
                .ToListAsync();

            return Ok(ApiResponse<object>.Success(leaderboard, "Lấy bảng xếp hạng thành công"));
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

            TimeZoneInfo vietnamTz = TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
            DateTime todayVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTz).Date;
            
            DateTime todayStartUtc = TimeZoneInfo.ConvertTimeToUtc(todayVn, vietnamTz);
            DateTime todayEndUtc = TimeZoneInfo.ConvertTimeToUtc(todayVn.AddDays(1), vietnamTz);

            List<FoodLog> existingLogs = await _context.FoodLogs
                .Where(f => f.UserId == userId && f.LogDate >= todayStartUtc && f.LogDate < todayEndUtc)
                .ToListAsync();
            _context.FoodLogs.RemoveRange(existingLogs);

            FoodLog log = new FoodLog
            {
                UserId = userId,
                FoodItemId = foodItem.Id,
                MealTypeId = 1, // Breakfast
                LogDate = todayStartUtc, // Save as UTC midnight of today local
                QuantityG = (targetKcal / 500m) * 100m + 10m,
                CaloriesKcal = targetKcal + 100m,
                ProteinG = 30m,
                CarbsG = 100m,
                FatG = 15m
            };

            _context.FoodLogs.Add(log);

            // Instant Evaluation for today
            if (log.CaloriesKcal >= targetKcal)
            {
                bool isLoggedToday = streak.LastLogDate.HasValue && TimeZoneInfo.ConvertTimeFromUtc(streak.LastLogDate.Value, vietnamTz).Date >= todayVn;
                if (!isLoggedToday)
                {
                    // Check if yesterday was logged OR frozen
                    DateTime yesterdayVn = todayVn.AddDays(-1);
                    DateTime yesterdayStartUtc = TimeZoneInfo.ConvertTimeToUtc(yesterdayVn, vietnamTz);
                    DateTime yesterdayEndUtc = todayStartUtc;

                    bool loggedYesterday = await _context.FoodLogs
                        .AnyAsync(f => f.UserId == userId && f.LogDate >= yesterdayStartUtc && f.LogDate < yesterdayEndUtc);
                    bool frozenYesterday = await _context.StreakFreezeTransactions
                        .AnyAsync(f => f.UserId == userId && f.FreezeDate.Date == yesterdayVn);

                    if (loggedYesterday || frozenYesterday || streak.CurrentStreak == 0)
                    {
                        streak.CurrentStreak += 1;
                    }
                    else
                    {
                        streak.CurrentStreak = 1;
                    }

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
