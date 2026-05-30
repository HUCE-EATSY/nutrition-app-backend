namespace nutrition_app_backend.DTOs.Admin;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public byte Role { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVip { get; set; } // We can calculate this or mock it
}
