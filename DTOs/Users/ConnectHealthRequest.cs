using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.DTOs.Users;

public class ConnectHealthRequest
{
    [Required]
    [EnumDataType(typeof(HealthProvider))]
    [JsonPropertyName("provider")]
    public HealthProvider Provider { get; set; }
}
