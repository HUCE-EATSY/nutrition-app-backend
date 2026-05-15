namespace nutrition_app_backend.Models.Users;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string HashedToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; } = null!;
    
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsValid => !IsRevoked && !IsExpired;
}
