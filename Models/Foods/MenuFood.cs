using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nutrition_app_backend.Models.Foods;

public class MenuFood
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public Guid FoodItemId { get; set; }
    
    // Khối lượng thực phẩm trong thực đơn (gram)
    [Column(TypeName = "decimal(8, 2)")]
    public decimal QuantityG { get; set; }

    // Navigation properties
    public Menu Menu { get; set; } = null!;
    public FoodItem FoodItem { get; set; } = null!;
}
