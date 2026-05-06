using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Diary;
using nutrition_app_backend.Models.Diary;

namespace nutrition_app_backend.Services.Diary;

public class DiaryService : IDiaryService
{
    private readonly WaoDbContext _db;

    public DiaryService(WaoDbContext db)
    {
        _db = db;
    }

    // ── GET summary ───────────────────────────────────────────────────────────
    public async Task<DiaryDaySummaryResponse> GetDaySummaryAsync(Guid userId, string dateISO)
    {
        // Lấy mục tiêu calo/macro từ goal đang active (IsActive = true)
        var goal = await _db.UserGoals
            .Where(g => g.UserId == userId && g.IsActive)
            .OrderByDescending(g => g.CreatedAt)
            .FirstOrDefaultAsync();

        decimal targetCal    = goal?.TargetCalories  ?? 2000;
        decimal targetProtein = goal?.TargetProteinG ?? 120;
        decimal targetCarb   = goal?.TargetCarbsG    ?? 180;
        decimal targetFat    = goal?.TargetFatG      ?? 60;

        // Lấy tất cả entries của ngày
        var entries = await _db.DiaryEntries
            .Where(e => e.UserId == userId && e.DateISO == dateISO)
            .OrderBy(e => e.Hour)
            .ToListAsync();

        // Tổng consumed
        decimal consumedCal     = entries.Sum(e => e.TotalCalories);
        decimal consumedProtein = entries.Sum(e => e.ProteinGram);
        decimal consumedCarb    = entries.Sum(e => e.CarbGram);
        decimal consumedFat     = entries.Sum(e => e.FatGram);

        // Group theo giờ → 17 slots (7h-23h)
        var slots = Enumerable.Range(7, 17).Select(hour =>
        {
            var hourEntries = entries
                .Where(e => e.Hour == hour)
                .Select(e => new DiaryEntrySlotItem(
                    e.Id,
                    e.FoodName,
                    e.TotalCalories,
                    e.ProteinGram,
                    e.CarbGram,
                    e.FatGram,
                    "meal"
                ))
                .ToList();

            return new DiaryHourSlotResponse(hour, hourEntries);
        }).ToList();

        return new DiaryDaySummaryResponse(
            dateISO,
            targetCal,
            consumedCal,
            targetProtein,
            consumedProtein,
            targetCarb,
            consumedCarb,
            targetFat,
            consumedFat,
            slots
        );
    }

    // ── ADD entry ─────────────────────────────────────────────────────────────
    public async Task<DiaryEntryResponse> AddEntryAsync(Guid userId, CreateDiaryEntryRequest req)
    {
        var entry = new DiaryEntry
        {
            Id            = Guid.NewGuid(),
            UserId        = userId,
            FoodId        = req.FoodId,
            FoodName      = req.FoodName.Trim(),
            DateISO       = req.DateISO,
            Hour          = req.Hour,
            QuantityG     = req.QuantityG,
            TotalCalories = req.TotalCalories,
            ProteinGram   = req.ProteinGram,
            CarbGram      = req.CarbGram,
            FatGram       = req.FatGram,
            LoggedAt      = DateTime.UtcNow,
        };

        _db.DiaryEntries.Add(entry);
        await _db.SaveChangesAsync();
        return ToEntryResponse(entry);
    }

    // ── DELETE entry ──────────────────────────────────────────────────────────
    public async Task DeleteEntryAsync(Guid userId, Guid entryId)
    {
        var entry = await _db.DiaryEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId)
            ?? throw new KeyNotFoundException("Không tìm thấy bữa ăn");

        _db.DiaryEntries.Remove(entry);
        await _db.SaveChangesAsync();
    }

    // ── ADD exercise ──────────────────────────────────────────────────────────
    public async Task<ExerciseLogResponse> AddExerciseAsync(Guid userId, CreateExerciseRequest req)
    {
        var log = new ExerciseLog
        {
            Id              = Guid.NewGuid(),
            UserId          = userId,
            ActivityId      = req.ActivityId,
            ActivityLabel   = req.ActivityLabel,
            DateISO         = req.DateISO,
            Hour            = req.Hour,
            DurationMinutes = req.DurationMinutes,
            CaloriesBurned  = req.CaloriesBurned,
            LoggedAt        = DateTime.UtcNow,
        };

        _db.ExerciseLogs.Add(log);
        await _db.SaveChangesAsync();
        return ToExerciseResponse(log);
    }

    // ── GET exercises ─────────────────────────────────────────────────────────
    public async Task<List<ExerciseLogResponse>> GetExercisesAsync(Guid userId, string dateISO)
    {
        return await _db.ExerciseLogs
            .Where(e => e.UserId == userId && e.DateISO == dateISO)
            .OrderBy(e => e.Hour)
            .Select(e => ToExerciseResponse(e))
            .ToListAsync();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static DiaryEntryResponse ToEntryResponse(DiaryEntry e) => new(
        e.Id, e.FoodId, e.FoodName, e.DateISO, e.Hour,
        e.QuantityG, e.TotalCalories, e.ProteinGram, e.CarbGram, e.FatGram, e.LoggedAt
    );

    private static ExerciseLogResponse ToExerciseResponse(ExerciseLog e) => new(
        e.Id, e.ActivityId, e.ActivityLabel, e.DateISO, e.Hour,
        e.DurationMinutes, e.CaloriesBurned, e.LoggedAt
    );
}
