namespace nutrition_app_backend.DTOs.Admin;

public class AdminFoodStatsDto
{
    public int Total { get; set; }
    public int Visible { get; set; }
    public int Hidden { get; set; }
    public int Categories { get; set; }
}

public class AdminFoodCategoryDto
{
    public byte Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FoodCount { get; set; }
}
