using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Users;

namespace nutrition_app_backend.Services.Streak;

public class StreakService : IStreakService
{
    private readonly WaoDbContext _context;

    public StreakService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task ProcessStreaksAsync(CancellationToken cancellationToken = default)
    {
        // Cron runs at 23:59 VN time — check TODAY's logs in VN time
        var vietnamTz = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh");
        var todayVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTz).Date;
        var todayStart = todayVn;
        var todayEnd = todayVn.AddDays(1);

        var streaks = await _context.UserStreaks
            .Include(s => s.User)
            .ToListAsync(cancellationToken);

        foreach (var streak in streaks)
        {
            // Idempotency: skip if already processed today (VN date)
            if (streak.LastLogDate.HasValue &&
                TimeZoneInfo.ConvertTimeFromUtc(streak.LastLogDate.Value, vietnamTz).Date == todayVn)
            {
                continue;
            }

            // Rule: Streak chỉ được tính khi log đủ ngưỡng 50% BMR
            var activeGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.UserId == streak.UserId && g.IsActive, cancellationToken);
            decimal bmrThreshold = (activeGoal?.BmrKcal ?? 1600m) * 0.5m;

            var todayCalories = await _context.FoodLogs
                .Where(f => f.UserId == streak.UserId && f.LogDate >= todayStart && f.LogDate < todayEnd)
                .SumAsync(f => (decimal?)f.CaloriesKcal, cancellationToken) ?? 0m;

            bool isEligibleToday = todayCalories >= bmrThreshold;

            if (isEligibleToday)
            {
                // Ghi ăn đủ calo hôm nay → tăng/duy trì streak
                streak.CurrentStreak += 1;
                if (streak.CurrentStreak > streak.LongestStreak)
                    streak.LongestStreak = streak.CurrentStreak;
                streak.LastLogDate = DateTime.UtcNow;
            }
            else
            {
                // Không ghi ăn → kiểm tra freeze (tối đa 2 khiên/tuần)
                var alreadyFrozen = await _context.StreakFreezeTransactions
                    .AnyAsync(f => f.UserId == streak.UserId && f.FreezeDate.Date == todayVn, cancellationToken);

                if (!alreadyFrozen && streak.FreezeCount > 0)
                {
                    // Kiểm tra: tuần này đã dùng bao nhiêu khiên? (tối đa 2/tuần)
                    var startOfWeekVn = todayVn.AddDays(-(todayVn.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)todayVn.DayOfWeek - 1));
                    var freezesThisWeek = await _context.StreakFreezeTransactions
                        .CountAsync(f => f.UserId == streak.UserId && f.FreezeDate >= startOfWeekVn && f.FreezeDate <= todayVn,
                            cancellationToken);

                    if (freezesThisWeek < 2)
                    {
                        // Auto-dùng freeze (source=1)
                        streak.FreezeCount -= 1;
                        var trans = new StreakFreezeTransaction
                        {
                            Id = Guid.NewGuid(),
                            UserId = streak.UserId,
                            FreezeDate = todayVn,
                            Source = 1 // 1 = Auto (cron), 2 = Manual
                        };
                        _context.StreakFreezeTransactions.Add(trans);
                    }
                    else
                    {
                        // Đã dùng 2 khiên tuần này → reset streak
                        streak.CurrentStreak = 0;
                    }
                }
                else if (!alreadyFrozen)
                {
                    // Không có freeze → reset streak
                    streak.CurrentStreak = 0;
                }
                // nếu alreadyFrozen => đã freeze rồi, không làm gì thêm (idempotent)
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<StreakLeaderboardDto>> GetLeaderboardAsync(int top = 50)
    {
        var streaks = await _context.UserStreaks
            .Include(s => s.User)
            .ThenInclude(u => u.AuthProviders)
            .OrderByDescending(s => s.CurrentStreak)
            .Take(top)
            .ToListAsync();

        return streaks.Select(s => new StreakLeaderboardDto
        {
            UserId = s.UserId,
            DisplayName = s.User.AuthProviders.FirstOrDefault()?.Email ?? "Anonymous",
            AvatarUrl = null,
            CurrentStreak = s.CurrentStreak,
            LongestStreak = s.LongestStreak
        }).ToList();
    }

    public async Task AdjustStreakForTestAsync(Guid userId, int streakToAdd, int freezeToAdd)
    {
        var streak = await _context.UserStreaks.FirstOrDefaultAsync(s => s.UserId == userId);
        if (streak == null)
        {
            streak = new UserStreak
            {
                UserId = userId,
                CurrentStreak = Math.Max(0, streakToAdd),
                LongestStreak = Math.Max(0, streakToAdd),
                FreezeCount = Math.Max(0, freezeToAdd),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserStreaks.Add(streak);
        }
        else
        {
            streak.CurrentStreak = Math.Max(0, streak.CurrentStreak + streakToAdd);
            streak.FreezeCount = Math.Max(0, streak.FreezeCount + freezeToAdd);
            if (streak.CurrentStreak > streak.LongestStreak)
                streak.LongestStreak = streak.CurrentStreak;

            streak.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
