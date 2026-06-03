namespace nutrition_app_backend.Services.Admin;

using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Exercises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AdminExerciseService : IAdminExerciseService
{
    private readonly WaoDbContext _dbContext;

    public AdminExerciseService(WaoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<AdminExerciseDto>> GetAllExercisesAsync(int page, int pageSize, string? search, int? categoryId = null, string? status = null)
    {
        var query = _dbContext.Exercises.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(e => e.NameVi.ToLower().Contains(searchLower) || (e.NameEn != null && e.NameEn.ToLower().Contains(searchLower)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            var statusInt = status.ToLower() == "visible" ? 1 : status.ToLower() == "hidden" ? 0 : -1;
            if (statusInt >= 0)
                query = query.Where(e => e.Status == statusInt);
        }

        var exercises = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new AdminExerciseDto
            {
                Id = e.Id,
                CategoryId = e.CategoryId,
                NameVi = e.NameVi,
                NameEn = e.NameEn,
                Description = e.Description,
                MetValue = e.MetValue,
                Unit = e.Unit,
                IconUrl = e.IconUrl,
                Status = e.Status,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return exercises;
    }

    public async Task<AdminExerciseDto> CreateExerciseAsync(AdminExerciseCreateDto dto)
    {
        var exercise = new Exercise
        {
            Id = Guid.NewGuid(),
            CategoryId = dto.CategoryId,
            NameVi = dto.NameVi,
            NameEn = dto.NameEn,
            Description = dto.Description,
            MetValue = dto.MetValue,
            Unit = dto.Unit ?? "minutes",
            IconUrl = dto.IconUrl,
            Status = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Exercises.Add(exercise);
        await _dbContext.SaveChangesAsync();

        return new AdminExerciseDto
        {
            Id = exercise.Id,
            CategoryId = exercise.CategoryId,
            NameVi = exercise.NameVi,
            NameEn = exercise.NameEn,
            Description = exercise.Description,
            MetValue = exercise.MetValue,
            Unit = exercise.Unit,
            IconUrl = exercise.IconUrl,
            Status = exercise.Status,
            CreatedAt = exercise.CreatedAt
        };
    }

    public async Task<AdminExerciseDto?> UpdateExerciseAsync(Guid id, AdminExerciseUpdateDto dto)
    {
        var exercise = await _dbContext.Exercises.FindAsync(id);
        if (exercise == null) throw new NotFoundException("Exercise not found.");

        if (dto.CategoryId.HasValue) exercise.CategoryId = dto.CategoryId.Value;
        if (dto.NameVi != null) exercise.NameVi = dto.NameVi;
        if (dto.NameEn != null) exercise.NameEn = dto.NameEn;
        if (dto.Description != null) exercise.Description = dto.Description;
        if (dto.MetValue.HasValue) exercise.MetValue = dto.MetValue.Value;
        if (dto.Unit != null) exercise.Unit = dto.Unit;
        if (dto.IconUrl != null) exercise.IconUrl = dto.IconUrl;

        exercise.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return new AdminExerciseDto
        {
            Id = exercise.Id,
            CategoryId = exercise.CategoryId,
            NameVi = exercise.NameVi,
            NameEn = exercise.NameEn,
            Description = exercise.Description,
            MetValue = exercise.MetValue,
            Unit = exercise.Unit,
            IconUrl = exercise.IconUrl,
            Status = exercise.Status,
            CreatedAt = exercise.CreatedAt
        };
    }

    public async Task<bool> DeleteExerciseAsync(Guid id)
    {
        var exercise = await _dbContext.Exercises.FindAsync(id);
        if (exercise == null) throw new NotFoundException("Exercise not found.");

        _dbContext.Exercises.Remove(exercise);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleVisibilityAsync(Guid id)
    {
        var exercise = await _dbContext.Exercises.FindAsync(id);
        if (exercise == null) throw new NotFoundException("Exercise not found.");

        exercise.Status = (byte)(exercise.Status == 1 ? 0 : 1);
        exercise.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<object> GetStatsAsync()
    {
        var total = await _dbContext.Exercises.CountAsync();
        var visible = await _dbContext.Exercises.CountAsync(e => e.Status == 1);
        var categories = await _dbContext.ExerciseCategories.CountAsync();

        return new
        {
            total,
            visible,
            categories
        };
    }

    public async Task<IEnumerable<object>> GetCategoriesAsync()
    {
        var categories = await _dbContext.ExerciseCategories
            .Select(c => new
            {
                id = c.Id,
                name = c.NameVi,
                nameEn = c.NameEn,
                exerciseCount = _dbContext.Exercises.Count(e => e.CategoryId == c.Id)
            })
            .OrderBy(c => c.id)
            .ToListAsync();

        return categories.Cast<object>();
    }
}
