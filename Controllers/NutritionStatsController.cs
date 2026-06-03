using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nutrition_app_backend.Controllers
{
    /// <summary>
    /// API thống kê dinh dưỡng nâng cao: tuần, tháng, mục tiêu, cân bằng calo.
    /// </summary>
    [ApiController]
    [Route("api/nutrition-stats")]
    [Authorize]
    public class NutritionStatsController : ControllerBase
    {
        private readonly WaoDbContext _context;

        public NutritionStatsController(WaoDbContext context)
        {
            _context = context;
        }

        // ===================================================
        // GET /api/nutrition-stats/weekly?date=2026-06-03
        // Thống kê dinh dưỡng 7 ngày kể từ ngày được truyền vào
        // ===================================================
        [HttpGet("weekly")]
        public async Task<ActionResult<ApiResponse<object>>> GetWeeklyStats([FromQuery] DateOnly date)
        {
            Guid userId = User.GetUserId();

            // Xác định đầu tuần (Thứ 2) và cuối tuần (Chủ Nhật)
            int dayOfWeek = (int)date.DayOfWeek;
            int offsetToMonday = dayOfWeek == 0 ? -6 : 1 - dayOfWeek;
            DateOnly startOfWeek = date.AddDays(offsetToMonday);
            DateOnly endOfWeek = startOfWeek.AddDays(6);

            DateTime startDt = startOfWeek.ToDateTime(TimeOnly.MinValue);
            DateTime endDt = endOfWeek.ToDateTime(TimeOnly.MaxValue);

            var logs = await _context.FoodLogs
                .Where(l => l.UserId == userId && l.LogDate >= startDt && l.LogDate <= endDt)
                .ToListAsync();

            var activeGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);

            var dailyStats = new List<object>();
            decimal totalWeekCalories = 0;
            decimal totalWeekProtein = 0;
            decimal totalWeekCarbs = 0;
            decimal totalWeekFat = 0;
            int daysAchievedGoal = 0;

            for (int i = 0; i < 7; i++)
            {
                DateOnly day = startOfWeek.AddDays(i);
                var dayLogs = logs.Where(l => DateOnly.FromDateTime(l.LogDate) == day).ToList();
                decimal cal = dayLogs.Sum(l => l.CaloriesKcal);
                decimal prot = dayLogs.Sum(l => l.ProteinG);
                decimal carb = dayLogs.Sum(l => l.CarbsG);
                decimal fat = dayLogs.Sum(l => l.FatG);

                totalWeekCalories += cal;
                totalWeekProtein += prot;
                totalWeekCarbs += carb;
                totalWeekFat += fat;

                bool achieved = activeGoal != null && cal >= activeGoal.TargetCalories * 0.8m && cal <= activeGoal.TargetCalories * 1.2m;
                if (achieved) daysAchievedGoal++;

                dailyStats.Add(new
                {
                    date = day.ToString("yyyy-MM-dd"),
                    dayName = GetVietnameseDayName(day.DayOfWeek),
                    calories = Math.Round(cal, 1),
                    proteinG = Math.Round(prot, 1),
                    carbsG = Math.Round(carb, 1),
                    fatG = Math.Round(fat, 1),
                    hasLog = dayLogs.Count > 0,
                    achievedGoal = achieved,
                    caloriesTarget = activeGoal?.TargetCalories ?? 0,
                    caloriePct = activeGoal != null && activeGoal.TargetCalories > 0
                        ? Math.Round(cal / activeGoal.TargetCalories * 100, 1)
                        : 0
                });
            }

            var result = new
            {
                weekStart = startOfWeek.ToString("yyyy-MM-dd"),
                weekEnd = endOfWeek.ToString("yyyy-MM-dd"),
                totalCalories = Math.Round(totalWeekCalories, 1),
                totalProteinG = Math.Round(totalWeekProtein, 1),
                totalCarbsG = Math.Round(totalWeekCarbs, 1),
                totalFatG = Math.Round(totalWeekFat, 1),
                avgDailyCalories = Math.Round(totalWeekCalories / 7, 1),
                daysAchievedGoal = daysAchievedGoal,
                targetCaloriesPerDay = activeGoal?.TargetCalories ?? 0,
                days = dailyStats
            };

            return Ok(ApiResponse<object>.Success(result, "Thống kê tuần thành công"));
        }

        // ===================================================
        // GET /api/nutrition-stats/monthly?year=2026&month=6
        // Thống kê dinh dưỡng theo tháng
        // ===================================================
        [HttpGet("monthly")]
        public async Task<ActionResult<ApiResponse<object>>> GetMonthlyStats(
            [FromQuery] int year, [FromQuery] int month)
        {
            if (month < 1 || month > 12)
                return BadRequest(ApiResponse<object>.Fail("Tháng không hợp lệ (1-12)"));

            Guid userId = User.GetUserId();

            DateOnly firstDay = new DateOnly(year, month, 1);
            DateOnly lastDay = firstDay.AddMonths(1).AddDays(-1);

            DateTime startDt = firstDay.ToDateTime(TimeOnly.MinValue);
            DateTime endDt = lastDay.ToDateTime(TimeOnly.MaxValue);

            var logs = await _context.FoodLogs
                .Where(l => l.UserId == userId && l.LogDate >= startDt && l.LogDate <= endDt)
                .ToListAsync();

            var activeGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);

            int totalDays = lastDay.Day;
            int daysLogged = logs.Select(l => l.LogDate.Date).Distinct().Count();
            decimal totalCal = logs.Sum(l => l.CaloriesKcal);
            decimal totalProt = logs.Sum(l => l.ProteinG);
            decimal totalCarb = logs.Sum(l => l.CarbsG);
            decimal totalFat = logs.Sum(l => l.FatG);

            // Thống kê theo từng tuần trong tháng
            var weeklyBreakdown = new List<object>();
            DateOnly cursor = firstDay;
            int weekNum = 1;
            while (cursor <= lastDay)
            {
                DateOnly weekEnd = cursor.AddDays(6);
                if (weekEnd > lastDay) weekEnd = lastDay;
                var weekLogs = logs.Where(l =>
                {
                    DateOnly d = DateOnly.FromDateTime(l.LogDate);
                    return d >= cursor && d <= weekEnd;
                }).ToList();

                weeklyBreakdown.Add(new
                {
                    week = weekNum,
                    from = cursor.ToString("yyyy-MM-dd"),
                    to = weekEnd.ToString("yyyy-MM-dd"),
                    totalCalories = Math.Round(weekLogs.Sum(l => l.CaloriesKcal), 1),
                    totalProteinG = Math.Round(weekLogs.Sum(l => l.ProteinG), 1),
                    totalCarbsG = Math.Round(weekLogs.Sum(l => l.CarbsG), 1),
                    totalFatG = Math.Round(weekLogs.Sum(l => l.FatG), 1),
                    daysLogged = weekLogs.Select(l => l.LogDate.Date).Distinct().Count()
                });
                cursor = weekEnd.AddDays(1);
                weekNum++;
            }

            var result = new
            {
                year = year,
                month = month,
                totalDays = totalDays,
                daysLogged = daysLogged,
                daysNotLogged = totalDays - daysLogged,
                logRate = Math.Round((decimal)daysLogged / totalDays * 100, 1),
                totalCalories = Math.Round(totalCal, 1),
                totalProteinG = Math.Round(totalProt, 1),
                totalCarbsG = Math.Round(totalCarb, 1),
                totalFatG = Math.Round(totalFat, 1),
                avgDailyCalories = daysLogged > 0 ? Math.Round(totalCal / daysLogged, 1) : 0,
                targetCaloriesPerDay = activeGoal?.TargetCalories ?? 0,
                weeks = weeklyBreakdown
            };

            return Ok(ApiResponse<object>.Success(result, "Thống kê tháng thành công"));
        }

        // ===================================================
        // GET /api/nutrition-stats/macro-breakdown?date=2026-06-03
        // Phân tích tỉ lệ macro (protein/carbs/fat) cho ngày
        // ===================================================
        [HttpGet("macro-breakdown")]
        public async Task<ActionResult<ApiResponse<object>>> GetMacroBreakdown([FromQuery] DateOnly date)
        {
            Guid userId = User.GetUserId();

            DateTime start = date.ToDateTime(TimeOnly.MinValue);
            DateTime end = start.AddDays(1);

            var logs = await _context.FoodLogs
                .Where(l => l.UserId == userId && l.LogDate >= start && l.LogDate < end)
                .ToListAsync();

            decimal totalCal = logs.Sum(l => l.CaloriesKcal);
            decimal totalProt = logs.Sum(l => l.ProteinG);
            decimal totalCarb = logs.Sum(l => l.CarbsG);
            decimal totalFat = logs.Sum(l => l.FatG);

            // Calo từ macro (Atwater: protein=4, carb=4, fat=9)
            decimal calFromProt = totalProt * 4;
            decimal calFromCarb = totalCarb * 4;
            decimal calFromFat = totalFat * 9;
            decimal totalMacroCal = calFromProt + calFromCarb + calFromFat;

            var result = new
            {
                date = date.ToString("yyyy-MM-dd"),
                totalCalories = Math.Round(totalCal, 1),
                totalProteinG = Math.Round(totalProt, 1),
                totalCarbsG = Math.Round(totalCarb, 1),
                totalFatG = Math.Round(totalFat, 1),
                macroCalories = new
                {
                    fromProtein = Math.Round(calFromProt, 1),
                    fromCarbs = Math.Round(calFromCarb, 1),
                    fromFat = Math.Round(calFromFat, 1)
                },
                macroPct = totalMacroCal > 0 ? new
                {
                    proteinPct = Math.Round(calFromProt / totalMacroCal * 100, 1),
                    carbsPct = Math.Round(calFromCarb / totalMacroCal * 100, 1),
                    fatPct = Math.Round(calFromFat / totalMacroCal * 100, 1)
                } : new { proteinPct = 0m, carbsPct = 0m, fatPct = 0m },
                mealBreakdown = logs
                    .GroupBy(l => l.MealTypeId)
                    .Select(g => new
                    {
                        mealTypeId = g.Key,
                        calories = Math.Round(g.Sum(x => x.CaloriesKcal), 1),
                        itemCount = g.Count()
                    }).ToList()
            };

            return Ok(ApiResponse<object>.Success(result, "Phân tích macro thành công"));
        }

        // ===================================================
        // GET /api/nutrition-stats/calorie-balance?from=...&to=...
        // Cân bằng calo: ăn vào vs tiêu hao (bước chân + bài tập)
        // ===================================================
        [HttpGet("calorie-balance")]
        public async Task<ActionResult<ApiResponse<object>>> GetCalorieBalance(
            [FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            if (to < from) return BadRequest(ApiResponse<object>.Fail("Ngày kết thúc phải sau ngày bắt đầu"));
            if ((to.DayNumber - from.DayNumber) > 31)
                return BadRequest(ApiResponse<object>.Fail("Khoảng thời gian tối đa 31 ngày"));

            Guid userId = User.GetUserId();

            DateTime startDt = from.ToDateTime(TimeOnly.MinValue);
            DateTime endDt = to.ToDateTime(TimeOnly.MaxValue);

            // Lấy food logs
            var foodLogs = await _context.FoodLogs
                .Where(l => l.UserId == userId && l.LogDate >= startDt && l.LogDate <= endDt)
                .ToListAsync();

            // Lấy step logs (calories burned from walking)
            var stepLogs = await _context.StepLogs
                .Where(l => l.UserId == userId && l.LogDate >= from && l.LogDate <= to)
                .ToListAsync();

            // Lấy exercise logs (calories burned)
            var exerciseLogs = await _context.ExerciseLogs
                .Where(l => l.UserId == userId && l.LogDate >= from && l.LogDate <= to)
                .ToListAsync();

            var activeGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive);

            decimal baseTdee = activeGoal?.TdeeKcal ?? 1800m;

            var dailyBalance = new List<object>();
            for (DateOnly d = from; d <= to; d = d.AddDays(1))
            {
                DateTime dayStart = d.ToDateTime(TimeOnly.MinValue);
                DateTime dayEnd = dayStart.AddDays(1);

                decimal calsIn = foodLogs
                    .Where(l => l.LogDate >= dayStart && l.LogDate < dayEnd)
                    .Sum(l => l.CaloriesKcal);

                decimal calsFromSteps = stepLogs
                    .Where(l => l.LogDate == d)
                    .Sum(l => l.CaloriesBurnedKcal);

                decimal calsFromExercise = exerciseLogs
                    .Where(l => l.LogDate == d)
                    .Sum(l => l.CaloriesBurned);

                decimal dayTotalOut = baseTdee + calsFromSteps + calsFromExercise;
                decimal netBalance = calsIn - dayTotalOut;

                dailyBalance.Add(new
                {
                    date = d.ToString("yyyy-MM-dd"),
                    caloriesIn = Math.Round(calsIn, 1),
                    caloriesOut = Math.Round(dayTotalOut, 1),
                    netBalance = Math.Round(netBalance, 1),
                    isDeficit = netBalance < 0,
                    breakdown = new
                    {
                        baseTdee = Math.Round(baseTdee, 1),
                        stepsCalories = Math.Round(calsFromSteps, 1),
                        exerciseCalories = Math.Round(calsFromExercise, 1)
                    }
                });
            }

            decimal totalIn = dailyBalance.Sum(d => (decimal)((dynamic)d).caloriesIn);
            decimal totalOut = dailyBalance.Sum(d => (decimal)((dynamic)d).caloriesOut);

            var result = new
            {
                from = from.ToString("yyyy-MM-dd"),
                to = to.ToString("yyyy-MM-dd"),
                totalCaloriesIn = Math.Round(totalIn, 1),
                totalCaloriesOut = Math.Round(totalOut, 1),
                totalNetBalance = Math.Round(totalIn - totalOut, 1),
                isOverallDeficit = (totalIn - totalOut) < 0,
                days = dailyBalance
            };

            return Ok(ApiResponse<object>.Success(result, "Cân bằng calo thành công"));
        }

        // ===================================================
        // GET /api/nutrition-stats/food-frequency?days=30
        // Thống kê món ăn được ghi nhiều nhất
        // ===================================================
        [HttpGet("food-frequency")]
        public async Task<ActionResult<ApiResponse<object>>> GetFoodFrequency([FromQuery] int days = 30)
        {
            if (days < 1 || days > 90) days = 30;

            Guid userId = User.GetUserId();
            DateTime since = DateTime.UtcNow.AddDays(-days);

            var topFoods = await _context.FoodLogs
                .Where(l => l.UserId == userId && l.LogDate >= since)
                .Include(l => l.FoodItem)
                .GroupBy(l => new { l.FoodItemId, l.FoodItem.NameVi })
                .Select(g => new
                {
                    foodItemId = g.Key.FoodItemId,
                    name = g.Key.NameVi,
                    count = g.Count(),
                    totalCalories = Math.Round(g.Sum(x => x.CaloriesKcal), 1),
                    avgQuantityG = Math.Round(g.Average(x => x.QuantityG), 1)
                })
                .OrderByDescending(x => x.count)
                .Take(10)
                .ToListAsync();

            var result = new
            {
                days = days,
                since = since.ToString("yyyy-MM-dd"),
                topFoods = topFoods
            };

            return Ok(ApiResponse<object>.Success(result, "Thống kê tần suất món ăn thành công"));
        }

        // ===================================================
        // Helper: Chuyển DayOfWeek sang tên tiếng Việt
        // ===================================================
        private static string GetVietnameseDayName(DayOfWeek dow) => dow switch
        {
            DayOfWeek.Monday => "T2",
            DayOfWeek.Tuesday => "T3",
            DayOfWeek.Wednesday => "T4",
            DayOfWeek.Thursday => "T5",
            DayOfWeek.Friday => "T6",
            DayOfWeek.Saturday => "T7",
            DayOfWeek.Sunday => "CN",
            _ => "?"
        };
    }
}
