using nutrition_app_backend.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace nutrition_app_backend.Models.Foods;

public class Menu
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    [MaxLength(255)]
    public string Name { get; set; } = null!;
    [MaxLength(1000)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<MenuFood> MenuFoods { get; set; } = new List<MenuFood>();
}
