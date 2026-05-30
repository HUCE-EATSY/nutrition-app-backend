using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Diaries;
using AutoMapper;

namespace nutrition_app_backend.Services.StepLog;

public class StepLogService : IStepLogService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;

    public StepLogService(WaoDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<StepLogResponse> UpsertAsync(Guid userId, UpsertStepLogRequest request)
    {
        var log = await _db.StepLogs
            .FirstOrDefaultAsync(s => s.UserId == userId && s.LogDate == request.LogDate);

        if (log != null)
        {
            log.Steps = request.Steps;
            log.StepGoal = request.StepGoal;
            if (request.Provider.HasValue)
            {
                log.Provider = request.Provider.Value;
            }
            if (request.CaloriesBurnedKcal.HasValue)
            {
                log.CaloriesBurnedKcal = request.CaloriesBurnedKcal.Value;
            }
            log.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            log = new Models.Diaries.StepLog
            {
                UserId = userId,
                LogDate = request.LogDate,
                Steps = request.Steps,
                StepGoal = request.StepGoal,
                Provider = request.Provider,
                CaloriesBurnedKcal = request.CaloriesBurnedKcal ?? 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.StepLogs.Add(log);
        }

        await _db.SaveChangesAsync();
        return _mapper.Map<StepLogResponse>(log);
    }

    public async Task<List<StepLogResponse>> GetTimelineAsync(Guid userId, DateOnly from, DateOnly to)
    {
        var logs = await _db.StepLogs
            .Where(s => s.UserId == userId && s.LogDate >= from && s.LogDate <= to)
            .OrderBy(s => s.LogDate)
            .ToListAsync();

        return _mapper.Map<List<StepLogResponse>>(logs);
    }
}
