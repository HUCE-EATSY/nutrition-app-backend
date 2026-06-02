namespace nutrition_app_backend.DTOs.Admin;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public int? VipPackageId { get; set; }
    public string? VipPackageName { get; set; }
    public DateTime? VipExpiresAt { get; set; }
}
