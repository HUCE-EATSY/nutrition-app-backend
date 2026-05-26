using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Users;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Enums;
using AutoMapper;

namespace nutrition_app_backend.Services.HealthConnection;

public class HealthConnectionService : IHealthConnectionService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;

    public HealthConnectionService(WaoDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<UserHealthConnectionResponse>> GetConnectionsAsync(Guid userId)
    {
        var connections = await _db.UserHealthConnections
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return _mapper.Map<List<UserHealthConnectionResponse>>(connections);
    }

    public async Task<UserHealthConnectionResponse> ConnectAsync(Guid userId, ConnectHealthRequest request)
    {
        var connection = await _db.UserHealthConnections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Provider == request.Provider);

        if (connection != null)
        {
            connection.Status = 1; // Connected
            connection.ConnectedAt = DateTime.UtcNow;
            connection.RevokedAt = null;
        }
        else
        {
            connection = new UserHealthConnection
            {
                UserId = userId,
                Provider = request.Provider,
                Status = 1,
                ConnectedAt = DateTime.UtcNow
            };
            _db.UserHealthConnections.Add(connection);
        }

        await _db.SaveChangesAsync();
        return _mapper.Map<UserHealthConnectionResponse>(connection);
    }

    public async Task DisconnectAsync(Guid userId, HealthProvider provider)
    {
        var connection = await _db.UserHealthConnections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Provider == provider);

        if (connection == null)
            throw new NotFoundException("Không tìm thấy kết nối ứng dụng sức khỏe.");

        connection.Status = 0; // Disconnected
        connection.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }
}
