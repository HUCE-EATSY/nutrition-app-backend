using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.Models.Streaks;

namespace nutrition_app_backend.Services.Streaks;

public class StreakService : IStreakService
{
    private readonly WaoDbContext _context;

    public StreakService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<UserStreak?> GetStreakAsync(Guid userId)
    {
        return await _context.Set<UserStreak>().FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<bool> FreezeStreakAsync(Guid userId, DateOnly targetDate)
    {
        var streak = await _context.Set<UserStreak>().FirstOrDefaultAsync(s => s.UserId == userId);
        if (streak == null || streak.FreezeCount <= 0) return false;

        var existingFreeze = await _context.Set<StreakFreezeTransaction>()
            .AnyAsync(f => f.UserId == userId && f.ProtectedDate == targetDate);
            
        if (existingFreeze) return false;

        streak.FreezeCount -= 1;
        
        var transaction = new StreakFreezeTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProtectedDate = targetDate,
            Source = 1 // or derived from plan
        };

        _context.Set<StreakFreezeTransaction>().Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }
}
