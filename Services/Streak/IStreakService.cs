using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nutrition_app_backend.Services.Streak;

public class StreakLeaderboardDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
}

public interface IStreakService
{
    Task ProcessStreaksAsync(CancellationToken cancellationToken = default);
    Task<List<StreakLeaderboardDto>> GetLeaderboardAsync(int top = 50);
    Task AdjustStreakForTestAsync(Guid userId, int streakToAdd, int freezeToAdd);
}
