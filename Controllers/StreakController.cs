using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Models.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace nutrition_app_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StreakController : ControllerBase
    {
        private readonly WaoDbContext _context;

        public StreakController(WaoDbContext context)
        {
            _context = context;
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

            var todayDt = DateTime.UtcNow.Date;
            
            var weeklyProgress = new bool[7];
            var dayOfWeek = (int)todayDt.DayOfWeek;
            var indexToday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

            var startOfWeekDt = todayDt.AddDays(-indexToday);
            var endOfTodayDt = todayDt.AddDays(1);

            var logs = await _context.FoodLogs
                .Where(f => f.UserId == userId && f.LogDate >= startOfWeekDt && f.LogDate < endOfTodayDt)
                .GroupBy(f => f.LogDate.Date)
                .Select(g => new { LogDate = g.Key, TotalCals = g.Sum(x => x.CaloriesKcal) })
                .ToListAsync();

            foreach (var log in logs)
            {
                var diff = (log.LogDate - startOfWeekDt).Days;
                if (diff >= 0 && diff < 7)
                {
                    if (log.TotalCals > 0)
                    {
                        weeklyProgress[diff] = true;
                    }
                }
            }
            
            var freezes = await _context.StreakFreezeTransactions
                .Where(f => f.UserId == userId && f.FreezeDate >= startOfWeekDt && f.FreezeDate <= todayDt)
                .Select(f => f.FreezeDate)
                .ToListAsync();
            
            foreach (var f in freezes)
            {
                var diff = (int)(f.Date - startOfWeekDt).TotalDays;
                if (diff >= 0 && diff < 7)
                {
                    weeklyProgress[diff] = true;
                }
            }

            var result = new
            {
                currentStreak = streak.CurrentStreak,
                longestStreak = streak.LongestStreak,
                freezeCount = streak.FreezeCount,
                weeklyProgress = weeklyProgress
            };

            return Ok(ApiResponse<object>.Success(result, "Lấy thông tin streak thành công"));
        }

        [HttpPost("freeze")]
        public async Task<ActionResult<ApiResponse<object>>> UseFreezeCard()
        {
            Guid userId = User.GetUserId();

            var streak = await _context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
            if (streak == null || streak.FreezeCount <= 0)
            {
                return BadRequest(ApiResponse<object>.Fail("Không đủ thẻ đóng băng"));
            }

            var yesterdayDt = DateTime.UtcNow.Date.AddDays(-1);
            var yesterdayEndDt = yesterdayDt.AddDays(1);

            var alreadyFrozen = await _context.StreakFreezeTransactions
                .AnyAsync(f => f.UserId == userId && f.FreezeDate.Date == yesterdayDt);

            if (alreadyFrozen)
            {
                return BadRequest(ApiResponse<object>.Fail("Bạn đã dùng thẻ đóng băng cho hôm qua rồi"));
            }

            var loggedYesterday = await _context.FoodLogs
                .AnyAsync(f => f.UserId == userId && f.LogDate >= yesterdayDt && f.LogDate < yesterdayEndDt);
            
            if (loggedYesterday)
            {
                return BadRequest(ApiResponse<object>.Fail("Hôm qua bạn đã hoàn thành mục tiêu, không cần đóng băng"));
            }

            streak.FreezeCount -= 1;

            var transaction = new StreakFreezeTransaction
            {
                UserId = userId,
                FreezeDate = yesterdayDt,
                Source = 2
            };

            _context.StreakFreezeTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Success(new { freezeCount = streak.FreezeCount }, "Đã sử dụng thẻ đóng băng"));
        }
    }
}
