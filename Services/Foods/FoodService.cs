using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Models.Foods;

namespace nutrition_app_backend.Services.Foods;

public class FoodService : IFoodService
{
    private readonly WaoDbContext _context;
    private readonly IMapper _mapper;

    public FoodService(WaoDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FoodDto>> GetAllAsync(string? category = null, string? search = null)
    {
        var query = _context.Foods.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(f => f.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(f => f.Name.Contains(search));
        }

        var foods = await query.ToListAsync();
        return _mapper.Map<IEnumerable<FoodDto>>(foods);
    }

    public async Task<FoodDto?> GetByIdAsync(int id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null) return null;

        return _mapper.Map<FoodDto>(food);
    }

    public async Task<FoodDto> CreateAsync(CreateFoodDto dto)
    {
        var food = _mapper.Map<Food>(dto);
        
        _context.Foods.Add(food);
        await _context.SaveChangesAsync();

        return _mapper.Map<FoodDto>(food);
    }

    public async Task<FoodDto?> UpdateAsync(int id, UpdateFoodDto dto)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null) return null;

        _mapper.Map(dto, food);
        
        await _context.SaveChangesAsync();

        return _mapper.Map<FoodDto>(food);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null) return false;

        _context.Foods.Remove(food);
        await _context.SaveChangesAsync();

        return true;
    }
}
