namespace nutrition_app_backend.Services.Admin;

using nutrition_app_backend.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public interface IAdminExerciseService
{
    Task<IEnumerable<AdminExerciseDto>> GetAllExercisesAsync(int page, int pageSize, string? search);
    Task<AdminExerciseDto> CreateExerciseAsync(AdminExerciseCreateDto dto);
    Task<AdminExerciseDto?> UpdateExerciseAsync(Guid id, AdminExerciseUpdateDto dto);
    Task<bool> DeleteExerciseAsync(Guid id);
    Task<bool> ToggleVisibilityAsync(Guid id);
}
