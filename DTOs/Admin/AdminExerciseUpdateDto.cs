namespace nutrition_app_backend.DTOs.Admin;

public class AdminExerciseUpdateDto
{
    public int? CategoryId { get; set; }
    public string? NameVi { get; set; }
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public decimal? MetValue { get; set; }
    public string? Unit { get; set; }
    public string? IconUrl { get; set; }
}
