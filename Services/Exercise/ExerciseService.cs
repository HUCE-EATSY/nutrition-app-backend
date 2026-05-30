using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Exercises;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Exercises;

namespace nutrition_app_backend.Services.Exercise;

public class ExerciseService : IExerciseService
{
    private readonly WaoDbContext _context;

    public ExerciseService(WaoDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExerciseCategoryResponse>> GetExerciseCategoriesAsync()
    {
        // Dùng Include() để tránh N+1 query
        var categories = await _context.ExerciseCategories
            .Include(c => c.Exercises.Where(e => e.Status == 1))
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        // Map to response DTOs
        var result = categories.Select(c => new ExerciseCategoryResponse
        {
            Id = c.Id,
            NameVi = c.NameVi,
            NameEn = c.NameEn,
            IconUrl = c.IconUrl,
            Exercises = c.Exercises
                .OrderBy(e => e.NameVi)
                .Select(e => new ExerciseResponse
                {
                    Id = e.Id,
                    CategoryId = e.CategoryId,
                    CategoryNameVi = c.NameVi,
                    NameVi = e.NameVi,
                    NameEn = e.NameEn,
                    Description = e.Description,
                    MetValue = e.MetValue,
                    Unit = e.Unit,
                    IconUrl = e.IconUrl
                }).ToList()
        }).ToList();

        return result;
    }

    public async Task<ExerciseResponse> GetExerciseByIdAsync(Guid exerciseId)
    {
        var exercise = await _context.Exercises
            .Include(e => e.Category)
            .Where(e => e.Id == exerciseId && e.Status == 1)
            .Select(e => new ExerciseResponse
            {
                Id = e.Id,
                CategoryId = e.CategoryId,
                CategoryNameVi = e.Category.NameVi,
                NameVi = e.NameVi,
                NameEn = e.NameEn,
                Description = e.Description,
                MetValue = e.MetValue,
                Unit = e.Unit,
                IconUrl = e.IconUrl
            })
            .FirstOrDefaultAsync();

        if (exercise == null)
            throw new NotFoundException("Exercise not found");

        return exercise;
    }

    public async Task<ExerciseLogResponse> CreateExerciseLogAsync(Guid userId, CreateExerciseLogRequest request)
    {
        // Kiểm tra exercise có tồn tại không
        var exercise = await _context.Exercises.FindAsync(request.ExerciseId);
        if (exercise == null || exercise.Status != 1)
            throw new NotFoundException("Exercise not found");

        // Lấy cân nặng hiện tại của user (mặc định 65kg nếu chưa có profile)
        var userProfile = await _context.UserProfiles.FindAsync(userId);
        var weightKg = userProfile?.WeightKg ?? 65m;

        // Tính calories đốt cháy: Calories = MET × Weight(kg) × Duration(hours)
        var durationHours = request.DurationMinutes / 60.0m;
        var caloriesBurned = exercise.MetValue * weightKg * durationHours;

        // Điều chỉnh theo cường độ
        caloriesBurned = request.Intensity switch
        {
            1 => caloriesBurned * 0.8m, // Nhẹ: 80%
            3 => caloriesBurned * 1.2m, // Nặng: 120%
            _ => caloriesBurned // Trung bình: 100%
        };

        var log = new ExerciseLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ExerciseId = request.ExerciseId,
            LogDate = request.LogDate,
            DurationMinutes = request.DurationMinutes,
            Intensity = request.Intensity,
            CaloriesBurned = Math.Round(caloriesBurned, 2),
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ExerciseLogs.Add(log);
        await _context.SaveChangesAsync();

        return new ExerciseLogResponse
        {
            Id = log.Id,
            ExerciseId = log.ExerciseId,
            ExerciseNameVi = exercise.NameVi,
            ExerciseNameEn = exercise.NameEn,
            ExerciseIconUrl = exercise.IconUrl,
            LogDate = log.LogDate,
            DurationMinutes = log.DurationMinutes,
            Intensity = log.Intensity,
            CaloriesBurned = log.CaloriesBurned,
            Notes = log.Notes,
            CreatedAt = log.CreatedAt
        };
    }

    public async Task<ExerciseLogResponse> GetExerciseLogByIdAsync(Guid userId, Guid logId)
    {
        var log = await _context.ExerciseLogs
            .Include(l => l.Exercise)
            .Where(l => l.Id == logId && l.UserId == userId)
            .Select(l => new ExerciseLogResponse
            {
                Id = l.Id,
                ExerciseId = l.ExerciseId,
                ExerciseNameVi = l.Exercise.NameVi,
                ExerciseNameEn = l.Exercise.NameEn,
                ExerciseIconUrl = l.Exercise.IconUrl,
                LogDate = l.LogDate,
                DurationMinutes = l.DurationMinutes,
                Intensity = l.Intensity,
                CaloriesBurned = l.CaloriesBurned,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (log == null)
            throw new NotFoundException("Exercise log not found");

        return log;
    }

    public async Task<ExerciseLogResponse> UpdateExerciseLogAsync(Guid userId, Guid logId, UpdateExerciseLogRequest request)
    {
        var log = await _context.ExerciseLogs
            .Include(l => l.Exercise)
            .FirstOrDefaultAsync(l => l.Id == logId && l.UserId == userId);

        if (log == null)
            throw new NotFoundException("Exercise log not found");

        // Cập nhật các trường nếu có
        if (request.DurationMinutes.HasValue || request.Intensity.HasValue)
        {
            if (request.DurationMinutes.HasValue)
                log.DurationMinutes = request.DurationMinutes.Value;

            if (request.Intensity.HasValue)
                log.Intensity = request.Intensity.Value;

            // Tính lại calories
            var userProfile = await _context.UserProfiles.FindAsync(userId);
            var weightKg = userProfile?.WeightKg ?? 65m;
            var durationHours = log.DurationMinutes / 60.0m;
            var caloriesBurned = log.Exercise.MetValue * weightKg * durationHours;

            caloriesBurned = log.Intensity switch
            {
                1 => caloriesBurned * 0.8m,
                3 => caloriesBurned * 1.2m,
                _ => caloriesBurned
            };

            log.CaloriesBurned = Math.Round(caloriesBurned, 2);
        }

        if (request.Notes != null)
            log.Notes = request.Notes;

        log.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ExerciseLogResponse
        {
            Id = log.Id,
            ExerciseId = log.ExerciseId,
            ExerciseNameVi = log.Exercise.NameVi,
            ExerciseNameEn = log.Exercise.NameEn,
            ExerciseIconUrl = log.Exercise.IconUrl,
            LogDate = log.LogDate,
            DurationMinutes = log.DurationMinutes,
            Intensity = log.Intensity,
            CaloriesBurned = log.CaloriesBurned,
            Notes = log.Notes,
            CreatedAt = log.CreatedAt
        };
    }

    public async Task DeleteExerciseLogAsync(Guid userId, Guid logId)
    {
        var log = await _context.ExerciseLogs
            .FirstOrDefaultAsync(l => l.Id == logId && l.UserId == userId);

        if (log == null)
            throw new NotFoundException("Exercise log not found");

        _context.ExerciseLogs.Remove(log);
        await _context.SaveChangesAsync();
    }

    public async Task<DailyExerciseSummaryResponse> GetDailyExerciseSummaryAsync(Guid userId, DateOnly date)
    {
        var logs = await _context.ExerciseLogs
            .Include(l => l.Exercise)
            .Where(l => l.UserId == userId && l.LogDate == date)
            .OrderBy(l => l.CreatedAt)
            .Select(l => new ExerciseLogResponse
            {
                Id = l.Id,
                ExerciseId = l.ExerciseId,
                ExerciseNameVi = l.Exercise.NameVi,
                ExerciseNameEn = l.Exercise.NameEn,
                ExerciseIconUrl = l.Exercise.IconUrl,
                LogDate = l.LogDate,
                DurationMinutes = l.DurationMinutes,
                Intensity = l.Intensity,
                CaloriesBurned = l.CaloriesBurned,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return new DailyExerciseSummaryResponse
        {
            Date = date,
            TotalDurationMinutes = logs.Sum(l => l.DurationMinutes),
            TotalCaloriesBurned = logs.Sum(l => l.CaloriesBurned),
            ExerciseCount = logs.Count,
            Logs = logs
        };
    }

    public async Task<List<ExerciseLogResponse>> GetExerciseLogsAsync(Guid userId, DateOnly? startDate, DateOnly? endDate)
    {
        var query = _context.ExerciseLogs
            .Include(l => l.Exercise)
            .Where(l => l.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(l => l.LogDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(l => l.LogDate <= endDate.Value);

        var logs = await query
            .OrderByDescending(l => l.LogDate)
            .ThenByDescending(l => l.CreatedAt)
            .Take(500) // Giới hạn tối đa 500 records
            .Select(l => new ExerciseLogResponse
            {
                Id = l.Id,
                ExerciseId = l.ExerciseId,
                ExerciseNameVi = l.Exercise.NameVi,
                ExerciseNameEn = l.Exercise.NameEn,
                ExerciseIconUrl = l.Exercise.IconUrl,
                LogDate = l.LogDate,
                DurationMinutes = l.DurationMinutes,
                Intensity = l.Intensity,
                CaloriesBurned = l.CaloriesBurned,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return logs;
    }
}
