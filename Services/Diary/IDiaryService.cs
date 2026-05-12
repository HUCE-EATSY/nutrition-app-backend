using nutrition_app_backend.DTOs.Diary;

namespace nutrition_app_backend.Services.Diary;

public interface IDiaryService
{
    /// <summary>Lấy tóm tắt nhật ký ăn uống trong ngày.</summary>
    Task<DiaryDaySummaryResponse> GetDaySummaryAsync(Guid userId, string dateISO);

    /// <summary>Thêm một bữa ăn vào nhật ký.</summary>
    Task<DiaryEntryResponse> AddEntryAsync(Guid userId, CreateDiaryEntryRequest request);

    /// <summary>Xóa một bữa ăn khỏi nhật ký.</summary>
    Task DeleteEntryAsync(Guid userId, Guid entryId);

    /// <summary>Ghi một bài tập vào nhật ký.</summary>
    Task<ExerciseLogResponse> AddExerciseAsync(Guid userId, CreateExerciseRequest request);

    /// <summary>Lấy danh sách bài tập trong ngày.</summary>
    Task<List<ExerciseLogResponse>> GetExercisesAsync(Guid userId, string dateISO);
}
