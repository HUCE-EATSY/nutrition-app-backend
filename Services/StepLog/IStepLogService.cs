using nutrition_app_backend.DTOs.Diaries;

namespace nutrition_app_backend.Services.StepLog;

public interface IStepLogService
{
    Task<StepLogResponse> UpsertAsync(Guid userId, UpsertStepLogRequest request);
    Task<List<StepLogResponse>> GetTimelineAsync(Guid userId, DateOnly from, DateOnly to);
}
