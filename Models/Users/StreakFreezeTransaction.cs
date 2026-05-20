using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace nutrition_app_backend.Models.Users;

public class StreakFreezeTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public DateOnly ProtectedDate { get; set; }
    
    /// <summary>
    /// 1 = System Cron (auto consume), 2 = Manual trigger
    /// </summary>
    public int Source { get; set; }
    
    [Column(TypeName = "timestamp(6)")]
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; } = null!;
}
