using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Diaries;
using nutrition_app_backend.Exceptions;
using AutoMapper;

namespace nutrition_app_backend.Services.WeightLog;

public class WeightLogService : IWeightLogService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;

    public WeightLogService(WaoDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a weight log entry.
    /// Unique constraint (user_id, log_date) will cause DbUpdateException if duplicate.
    /// Returns 409 with suggestion to use PUT to update.
    /// </summary>
    public async Task<WeightLogResponse> CreateAsync(Guid userId, CreateWeightLogRequest request)
    {
        // Check for existing log on the same date to give a clear error message
        var existingLog = await _db.WeightLogs
            .FirstOrDefaultAsync(w => w.UserId == userId && w.LogDate == request.LogDate);

        if (existingLog != null)
            throw new ConflictException(
                "Bạn đã ghi cân nặng cho ngày này rồi. Vui lòng dùng PUT để cập nhật.");

        var log = new Models.Diaries.WeightLog
        {
            UserId = userId,
            WeightKg = request.WeightKg,
            LogDate = request.LogDate,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
        };

        _db.WeightLogs.Add(log);
        await _db.SaveChangesAsync();

        return _mapper.Map<WeightLogResponse>(log);
    }

    /// <summary>
    /// Get weight log timeline within date range, ordered ascending by date.
    /// </summary>
    public async Task<List<WeightLogResponse>> GetTimelineAsync(Guid userId, DateOnly from, DateOnly to)
    {
        var logs = await _db.WeightLogs
            .Where(w => w.UserId == userId && w.LogDate >= from && w.LogDate <= to)
            .OrderBy(w => w.LogDate)
            .ToListAsync();

        return _mapper.Map<List<WeightLogResponse>>(logs);
    }

    /// <summary>
    /// Update an existing weight log (weight_kg and note only).
    /// Only the owner can update.
    /// </summary>
    public async Task<WeightLogResponse> UpdateAsync(Guid userId, ulong logId, UpdateWeightLogRequest request)
    {
        var log = await _db.WeightLogs.FindAsync(logId);

        if (log == null)
            throw new NotFoundException("Không tìm thấy log cân nặng.");

        if (log.UserId != userId)
            throw new ForbiddenException("Bạn không có quyền sửa log này.");

        log.WeightKg = request.WeightKg;
        log.Note = request.Note;

        await _db.SaveChangesAsync();

        return _mapper.Map<WeightLogResponse>(log);
    }
}
