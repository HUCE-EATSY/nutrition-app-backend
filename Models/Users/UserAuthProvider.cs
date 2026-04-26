namespace nutrition_app_backend.Models.Users;

public class UserAuthProvider
{
    public ulong Id { get; set; }
    public Guid UserId { get; set; }
    public string Provider { get; set; } = null!;
    public string ProviderUid { get; set; } = null!;
    public string? Email { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; } = null!;
}