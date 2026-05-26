using System.Text.Json.Serialization;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Users;

public class UserHealthConnectionResponse
{
    [JsonPropertyName("provider")]
    public HealthProvider Provider { get; set; }

    [JsonPropertyName("status")]
    public byte Status { get; set; }

    [JsonPropertyName("connected_at")]
    public DateTime? ConnectedAt { get; set; }

    [JsonPropertyName("revoked_at")]
    public DateTime? RevokedAt { get; set; }
}
