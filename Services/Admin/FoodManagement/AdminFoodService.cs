using AutoMapper;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Admin;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Services.Storage;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Services.Admin.FoodManagement;

public class AdminFoodService : IAdminFoodService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;
    private readonly IStorageService _storageService;

    public AdminFoodService(WaoDbContext db, IMapper mapper, IStorageService storageService)
    {
        _db = db;
        _mapper = mapper;
        _storageService = storageService;
    }

    private decimal CalculateCalories(decimal protein, decimal carbs, decimal fat)
    {
        return protein * 4 + carbs * 4 + fat * 9;
    }

    private async Task<FoodItem> GetFoodOrThrowAsync(Guid foodId)
    {
        var food = await _db.FoodItems
            .Include(f => f.Nutrition)
            .Include(f => f.Images)
            .FirstOrDefaultAsync(f => f.Id == foodId);
            
        if (food == null)
            throw new NotFoundException("Không tìm thấy món ăn.");
            
        return food;
    }

    public async Task<FoodDetailResponse> CreateOfficialFoodAsync(Guid adminId, CreateOfficialFoodRequest request)
    {
        var categoryExists = await _db.FoodCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new BusinessException("Danh mục không hợp lệ.", "400");

        var newFood = new FoodItem
        {
            Id = Guid.NewGuid(),
            NameVi = request.NameVi,
            NameEn = request.NameEn,
            CategoryId = request.CategoryId,
            Source = FoodSource.Official, // Official
            Status = FoodStatus.Approved, // Approved
            ServingSizeG = request.ServingSizeG,
            ServingUnitVi = request.ServingUnitVi,
            Barcode = request.Barcode,
            CreatedBy = adminId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.FoodItems.Add(newFood);

        if (request.Nutrition != null)
        {
            var calculatedCalories = CalculateCalories(request.Nutrition.ProteinG, request.Nutrition.CarbsG, request.Nutrition.FatG);
            
            var nutrition = new FoodNutrition
            {
                FoodItemId = newFood.Id,
                CaloriesKcal = calculatedCalories,
                ProteinG = request.Nutrition.ProteinG,
                CarbsG = request.Nutrition.CarbsG,
                FatG = request.Nutrition.FatG,
                FiberG = request.Nutrition.FiberG,
                SugarG = request.Nutrition.SugarG,
                SodiumMg = request.Nutrition.SodiumMg,
                UpdatedAt = DateTime.UtcNow
            };
            _db.FoodNutritions.Add(nutrition);
            newFood.Nutrition = nutrition;
        }

        await _db.SaveChangesAsync();

        if (request.Image != null)
        {
            var storagePath = await _storageService.UploadAsync(request.Image, "wao/foods");
            var newImage = new FoodItemImage
            {
                FoodItemId = newFood.Id,
                StoragePath = storagePath,
                StorageProvider = "cloudinary",
                CreatedAt = DateTime.UtcNow
            };
            _db.FoodItemImages.Add(newImage);
            await _db.SaveChangesAsync();

            newFood.ActiveImageId = newImage.Id;
            await _db.SaveChangesAsync();
            
            // Reload to map correctly with image
            newFood.ActiveImage = newImage;
        }

        return _mapper.Map<FoodDetailResponse>(newFood);
    }

    public async Task<FoodDetailResponse> UpdateFoodMetadataAsync(Guid adminId, Guid foodId, UpdateFoodMetadataRequest request)
    {
        var food = await GetFoodOrThrowAsync(foodId);

        var categoryExists = await _db.FoodCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new BusinessException("Danh mục không hợp lệ.", "400");

        food.NameVi = request.NameVi;
        food.NameEn = request.NameEn;
        food.CategoryId = request.CategoryId;
        food.ServingSizeG = request.ServingSizeG;
        food.ServingUnitVi = request.ServingUnitVi;
        food.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return _mapper.Map<FoodDetailResponse>(food);
    }

    public async Task DeleteFoodAsync(Guid adminId, Guid foodId)
    {
        var food = await _db.FoodItems
            .Include(f => f.Children)
            .FirstOrDefaultAsync(f => f.Id == foodId);
            
        if (food == null)
            throw new NotFoundException("Không tìm thấy món ăn.");

        var hasLogs = await _db.FoodLogs.AnyAsync(l => l.FoodItemId == foodId);
        if (hasLogs)
            throw new ConflictException("Không thể xóa món ăn đã có dữ liệu log của người dùng.");

        _db.FoodItems.Remove(food);
        await _db.SaveChangesAsync();
    }

    public async Task<FoodDetailResponse> AddOrUpdateNutritionAsync(Guid adminId, Guid foodId, CreateFoodNutritionDto request)
    {
        var food = await GetFoodOrThrowAsync(foodId);
        var calculatedCalories = CalculateCalories(request.ProteinG, request.CarbsG, request.FatG);

        if (food.Nutrition == null)
        {
            var nutrition = new FoodNutrition
            {
                FoodItemId = foodId,
                CaloriesKcal = calculatedCalories,
                ProteinG = request.ProteinG,
                CarbsG = request.CarbsG,
                FatG = request.FatG,
                FiberG = request.FiberG,
                SugarG = request.SugarG,
                SodiumMg = request.SodiumMg,
                UpdatedAt = DateTime.UtcNow
            };
            _db.FoodNutritions.Add(nutrition);
            food.Nutrition = nutrition;
        }
        else
        {
            food.Nutrition.CaloriesKcal = calculatedCalories;
            food.Nutrition.ProteinG = request.ProteinG;
            food.Nutrition.CarbsG = request.CarbsG;
            food.Nutrition.FatG = request.FatG;
            food.Nutrition.FiberG = request.FiberG;
            food.Nutrition.SugarG = request.SugarG;
            food.Nutrition.SodiumMg = request.SodiumMg;
            food.Nutrition.UpdatedAt = DateTime.UtcNow;
            
            _db.FoodNutritions.Update(food.Nutrition);
        }

        food.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return _mapper.Map<FoodDetailResponse>(food);
    }

    public async Task<object> UploadFoodImageAsync(Guid adminId, Guid foodId, IFormFile image)
    {
        await GetFoodOrThrowAsync(foodId);

        var storagePath = await _storageService.UploadAsync(image, "wao/foods");

        var newImage = new FoodItemImage
        {
            FoodItemId = foodId,
            StoragePath = storagePath,
            StorageProvider = "cloudinary",
            CreatedAt = DateTime.UtcNow
        };

        _db.FoodItemImages.Add(newImage);
        await _db.SaveChangesAsync();

        return new { image_id = newImage.Id, storage_path = newImage.StoragePath };
    }

    public async Task<FoodDetailResponse> SetActiveImageAsync(Guid adminId, Guid foodId, SetActiveImageRequest request)
    {
        var food = await GetFoodOrThrowAsync(foodId);

        var image = food.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image == null)
        {
            var dbImage = await _db.FoodItemImages.FirstOrDefaultAsync(i => i.Id == request.ImageId && i.FoodItemId == foodId);
            if (dbImage == null)
                throw new BusinessException("Ảnh không tồn tại hoặc không thuộc về món ăn này.", "400");
        }

        food.ActiveImageId = request.ImageId;
        food.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var updatedFood = await _db.FoodItems
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .FirstOrDefaultAsync(f => f.Id == foodId);

        return _mapper.Map<FoodDetailResponse>(updatedFood);
    }

    public async Task DeleteImageAsync(Guid adminId, Guid foodId, ulong imageId)
    {
        var food = await GetFoodOrThrowAsync(foodId);

        if (food.ActiveImageId == imageId)
            throw new ConflictException("Không thể xóa ảnh đang được đặt làm ảnh hiển thị chính. Hãy đổi ảnh khác trước.");

        var image = await _db.FoodItemImages.FirstOrDefaultAsync(i => i.Id == imageId && i.FoodItemId == foodId);
        if (image == null)
            throw new NotFoundException("Không tìm thấy ảnh.");

        _db.FoodItemImages.Remove(image);
        await _db.SaveChangesAsync();
    }

    private async Task<bool> IsCycleAsync(Guid parentId, Guid childId)
    {
        var visited = new HashSet<Guid>();
        var queue = new Queue<Guid>();
        queue.Enqueue(childId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == parentId) return true; // cycle detected

            if (visited.Add(current))
            {
                var children = await _db.FoodItemComponents
                    .Where(c => c.ParentFoodId == current)
                    .Select(c => c.ChildFoodId)
                    .ToListAsync();

                foreach (var child in children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        return false;
    }

    public async Task<FoodDetailResponse> AddComponentAsync(Guid adminId, Guid foodId, AddComponentRequest request)
    {
        if (foodId == request.ChildFoodId)
            throw new BusinessException("Không thể thêm chính món này làm thành phần con.", "400");

        var childFood = await _db.FoodItems.FirstOrDefaultAsync(f => f.Id == request.ChildFoodId);
        if (childFood == null)
            throw new NotFoundException("Không tìm thấy món ăn con.");

        if (childFood.Status != FoodStatus.Approved)
            throw new BusinessException("Món ăn con chưa được duyệt (status != FoodStatus.Approved).", "400");

        if (await IsCycleAsync(foodId, request.ChildFoodId))
            throw new BusinessException("Phát hiện vòng lặp thành phần (Parent-Child cycle).", "400");

        var component = new FoodItemComponent
        {
            ParentFoodId = foodId,
            ChildFoodId = request.ChildFoodId,
            QuantityG = request.QuantityG
        };

        _db.FoodItemComponents.Add(component);
        await _db.SaveChangesAsync();

        var food = await GetFoodOrThrowAsync(foodId);
        return _mapper.Map<FoodDetailResponse>(food);
    }

    public async Task DeleteComponentAsync(Guid adminId, Guid foodId, ulong componentId)
    {
        var component = await _db.FoodItemComponents
            .FirstOrDefaultAsync(c => c.Id == componentId && c.ParentFoodId == foodId);

        if (component == null)
            throw new NotFoundException("Không tìm thấy thành phần.");

        _db.FoodItemComponents.Remove(component);
        await _db.SaveChangesAsync();
    }

    public async Task<PaginatedResponse<FoodSearchResponse>> GetPendingFoodsAsync(int page, int pageSize)
    {
        var query = _db.FoodItems
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .Where(f => f.Status == FoodStatus.Pending)
            .OrderByDescending(f => f.CreatedAt)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(Math.Min(pageSize, 50))
            .ToListAsync();

        return new PaginatedResponse<FoodSearchResponse>
        {
            Items = _mapper.Map<List<FoodSearchResponse>>(items),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<FoodDetailResponse> ReviewCommunityFoodAsync(Guid adminId, Guid foodId, ReviewCommunityFoodRequest request)
    {
        var food = await GetFoodOrThrowAsync(foodId);

        if (food.Status != FoodStatus.Pending)
            throw new BusinessException("Món ăn này không ở trạng thái chờ duyệt.", "400");

        if (request.Approve)
        {
            food.Status = FoodStatus.Approved;
            if (food.Source == FoodSource.Community && food.Barcode.HasValue)
            {
                food.Source = FoodSource.BarcodeCommunity;
            }
        }
        else
        {
            food.Status = FoodStatus.Rejected;
        }

        food.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return _mapper.Map<FoodDetailResponse>(food);
    }
}
