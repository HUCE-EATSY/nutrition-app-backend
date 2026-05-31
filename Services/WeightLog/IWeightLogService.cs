using nutrition_app_backend.DTOs.Diaries;

namespace nutrition_app_backend.Services.WeightLog;

public interface IWeightLogService
{
    Task<WeightLogResponse> CreateAsync(Guid userId, CreateWeightLogRequest request);
    Task<WeightLogResponse> UpdateAsync(Guid userId, ulong logId, UpdateWeightLogRequest request);
    Task<List<WeightLogResponse>> GetTimelineAsync(Guid userId, DateOnly from, DateOnly to);
    Task<WeightLogResponse> UploadPhotoAsync(Guid userId, ulong logId, Microsoft.AspNetCore.Http.IFormFile file);
}
