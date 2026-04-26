namespace nutrition_app_backend.Models.Users;

public class User
{
    public Guid Id { get; set; }
    public byte Role { get; set; } = 1;
    public byte Status { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public ICollection<UserAuthProvider> AuthProviders { get; set; } = new List<UserAuthProvider>();
    public UserProfile? Profile { get; set; }
    public ICollection<UserGoal> Goals { get; set; } = new List<UserGoal>();
}