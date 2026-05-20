using System;
using System.Threading.Tasks;
using nutrition_app_backend.DTOs.Users;

namespace nutrition_app_backend.Services.Streak;

public interface IStreakService
{
    Task<StreakResponse> GetStreakAsync(Guid userId);
    Task<bool> FreezeStreakAsync(Guid userId);
}
