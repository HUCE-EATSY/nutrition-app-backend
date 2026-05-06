using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Services.Food;

public class FoodService : IFoodService
{
    private readonly WaoDbContext _db;

    public FoodService(WaoDbContext db)
    {
        _db = db;
    }

    public async Task<List<FoodResponse>> SearchFoodsAsync(string? query)
    {
        var q = _db.Foods.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.Trim().ToLower();
            q = q.Where(f => f.Name.ToLower().Contains(lower));
        }

        return await q
            .OrderBy(f => f.Name)
            .Take(20)
            .Select(f => ToResponse(f))
            .ToListAsync();
    }

    public async Task<FoodResponse> CreateFoodAsync(Guid userId, CreateFoodRequest request)
    {
        var food = new Models.Foods.Food
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            ImageUrl = request.ImageUrl,
            CaloriesPer100g = request.CaloriesPer100g,
            ProteinPer100g = request.ProteinPer100g,
            CarbPer100g = request.CarbPer100g,
            FatPer100g = request.FatPer100g,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Foods.Add(food);
        await _db.SaveChangesAsync();

        return ToResponse(food);
    }

    public async Task<FoodResponse?> GetFoodByIdAsync(Guid id)
    {
        var food = await _db.Foods.FindAsync(id);
        return food == null ? null : ToResponse(food);
    }

    // ── Helper ──────────────────────────────────────────────────────────────
    private static FoodResponse ToResponse(Models.Foods.Food f) => new(
        f.Id,
        f.Name,
        f.ImageUrl,
        f.CaloriesPer100g,
        f.ProteinPer100g,
        f.CarbPer100g,
        f.FatPer100g,
        f.CreatedAt
    );
}
