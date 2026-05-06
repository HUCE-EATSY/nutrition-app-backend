using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.DTOs.Diary;

// ── Diary Entry ───────────────────────────────────────────────────────────────

public record CreateDiaryEntryRequest(
    [Required] Guid FoodId,
    [Required][MaxLength(200)] string FoodName,
    [Required][RegularExpression(@"\d{4}-\d{2}-\d{2}")] string DateISO,
    [Range(0, 23)] int Hour,
    [Range(1, 9999)] decimal QuantityG,
    [Range(0, 99999)] decimal TotalCalories,
    [Range(0, 9999)] decimal ProteinGram,
    [Range(0, 9999)] decimal CarbGram,
    [Range(0, 9999)] decimal FatGram
);

public record DiaryEntryResponse(
    Guid Id,
    Guid FoodId,
    string FoodName,
    string DateISO,
    int Hour,
    decimal QuantityG,
    decimal TotalCalories,
    decimal ProteinGram,
    decimal CarbGram,
    decimal FatGram,
    DateTime LoggedAt
);

// ── Exercise ──────────────────────────────────────────────────────────────────

public record CreateExerciseRequest(
    [Required][MaxLength(50)] string ActivityId,
    [Required][MaxLength(100)] string ActivityLabel,
    [Required][RegularExpression(@"\d{4}-\d{2}-\d{2}")] string DateISO,
    [Range(0, 23)] int Hour,
    [Range(1, 600)] int DurationMinutes,
    [Range(0, 9999)] decimal CaloriesBurned
);

public record ExerciseLogResponse(
    Guid Id,
    string ActivityId,
    string ActivityLabel,
    string DateISO,
    int Hour,
    int DurationMinutes,
    decimal CaloriesBurned,
    DateTime LoggedAt
);

// ── Summary (trả về cho GET /api/diary?date=) ─────────────────────────────────

public record DiaryEntrySlotItem(
    Guid Id,
    string Title,       // = FoodName
    decimal Calories,
    decimal ProteinGram,
    decimal CarbGram,
    decimal FatGram,
    string Type         // "meal"
);

public record DiaryHourSlotResponse(
    int Hour,
    List<DiaryEntrySlotItem> Entries
);

public record DiaryDaySummaryResponse(
    string DateISO,
    decimal TargetCalories,
    decimal ConsumedCalories,
    decimal TargetProteinGram,
    decimal ConsumedProteinGram,
    decimal TargetCarbGram,
    decimal ConsumedCarbGram,
    decimal TargetFatGram,
    decimal ConsumedFatGram,
    List<DiaryHourSlotResponse> Slots
);
