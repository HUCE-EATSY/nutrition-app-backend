using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Services.HealthConnection;

public interface IHealthConnectionService
{
    Task<List<UserHealthConnectionResponse>> GetConnectionsAsync(Guid userId);
    Task<UserHealthConnectionResponse> ConnectAsync(Guid userId, ConnectHealthRequest request);
    Task DisconnectAsync(Guid userId, HealthProvider provider);
}
