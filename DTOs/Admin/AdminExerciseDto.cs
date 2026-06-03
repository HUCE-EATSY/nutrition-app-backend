namespace nutrition_app_backend.DTOs.Admin;

public class AdminExerciseDto
{
    public Guid Id { get; set; }
    public int CategoryId { get; set; }
    public string NameVi { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MetValue { get; set; }
    public string Unit { get; set; } = "minutes";
    public string? IconUrl { get; set; }
    public byte Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
