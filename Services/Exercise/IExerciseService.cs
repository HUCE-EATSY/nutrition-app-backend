using nutrition_app_backend.DTOs.Exercises;

namespace nutrition_app_backend.Services.Exercise;

public interface IExerciseService
{
    Task<List<ExerciseCategoryResponse>> GetExerciseCategoriesAsync();
    Task<ExerciseResponse> GetExerciseByIdAsync(Guid exerciseId);
    Task<ExerciseLogResponse> CreateExerciseLogAsync(Guid userId, CreateExerciseLogRequest request);
    Task<ExerciseLogResponse> GetExerciseLogByIdAsync(Guid userId, Guid logId);
    Task<ExerciseLogResponse> UpdateExerciseLogAsync(Guid userId, Guid logId, UpdateExerciseLogRequest request);
    Task DeleteExerciseLogAsync(Guid userId, Guid logId);
    Task<DailyExerciseSummaryResponse> GetDailyExerciseSummaryAsync(Guid userId, DateOnly date);
    Task<List<ExerciseLogResponse>> GetExerciseLogsAsync(Guid userId, DateOnly? startDate, DateOnly? endDate);
}
