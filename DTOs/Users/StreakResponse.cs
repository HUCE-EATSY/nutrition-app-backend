using System;

namespace nutrition_app_backend.DTOs.Users;

public class StreakResponse
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int FreezeCount { get; set; }
    public DateOnly? LastLogDate { get; set; }
}
