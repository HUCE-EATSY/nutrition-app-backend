using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Services.Storage;
using AutoMapper;

namespace nutrition_app_backend.Services.Food;

public class FoodService : IFoodService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;
    private readonly IStorageService _storage;

    public FoodService(WaoDbContext db, IMapper mapper, IStorageService storage)
    {
        _db = db;
        _mapper = mapper;
        _storage = storage;
    }

    /// <summary>
    /// Fulltext search using MySQL MATCH … AGAINST in BOOLEAN MODE.
    /// - Approved items (status=1) visible to everyone.
    /// - Pending items (status=0) visible only to the creator.
    /// </summary>
    public async Task<PaginatedResponse<FoodSearchResponse>> SearchAsync(FoodSearchRequest request, Guid? currentUserId)
    {
        var searchTerm = request.Q.Trim();
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new BusinessException("EMPTY_SEARCH", "Từ khóa tìm kiếm không được để trống.");

        // Append * for prefix matching in BOOLEAN MODE
        var booleanTerm = string.Join(" ", searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => $"+{w}*"));

        var userIdStr = currentUserId?.ToString() ?? "";
        var offset = (request.Page - 1) * request.PageSize;

        // Build WHERE clause dynamically
        var whereClause = @"
            MATCH(fi.name_vi, fi.name_en) AGAINST (@searchTerm IN BOOLEAN MODE)
            AND (fi.status = 1 OR (fi.status = 0 AND fi.created_by = @userId))";

        if (request.CategoryId.HasValue)
        {
            whereClause += " AND fi.category_id = @categoryId";
        }

        // Count query
        var countSql = $"SELECT COUNT(*) FROM food_items fi WHERE {whereClause}";

        await using var connection = _db.Database.GetDbConnection();
        await connection.OpenAsync();

        int totalCount;
        await using (var countCmd = connection.CreateCommand())
        {
            countCmd.CommandText = countSql;
            countCmd.Parameters.Add(new MySqlParameter("@searchTerm", booleanTerm));
            countCmd.Parameters.Add(new MySqlParameter("@userId", userIdStr));
            if (request.CategoryId.HasValue)
                countCmd.Parameters.Add(new MySqlParameter("@categoryId", request.CategoryId.Value));

            totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
        }

        // Data query — ranked by relevance
        var dataSql = $@"
            SELECT
                fi.id                AS Id,
                fi.name_vi           AS NameVi,
                fi.name_en           AS NameEn,
                fi.category_id       AS CategoryId,
                fi.source            AS Source,
                fi.serving_size_g    AS ServingSizeG,
                fi.serving_unit_vi   AS ServingUnitVi,
                fi.thumbnail_url     AS ThumbnailUrl,
                fi.active_image_id   AS ActiveImageId,
                fn.calories_kcal     AS CaloriesKcal,
                fii.storage_path     AS ImageStoragePath,
                fii.storage_provider AS ImageStorageProvider
            FROM food_items fi
            LEFT JOIN food_nutrition fn ON fn.food_item_id = fi.id
            LEFT JOIN food_item_images fii ON fi.source != 3 AND fii.id = fi.active_image_id
            WHERE {whereClause}
            ORDER BY MATCH(fi.name_vi, fi.name_en) AGAINST (@searchTerm2 IN BOOLEAN MODE) DESC
            LIMIT @limit OFFSET @offset";

        var items = new List<FoodSearchResponse>();

        await using (var dataCmd = connection.CreateCommand())
        {
            dataCmd.CommandText = dataSql;
            dataCmd.Parameters.Add(new MySqlParameter("@searchTerm", booleanTerm));
            dataCmd.Parameters.Add(new MySqlParameter("@searchTerm2", booleanTerm));
            dataCmd.Parameters.Add(new MySqlParameter("@userId", userIdStr));
            dataCmd.Parameters.Add(new MySqlParameter("@limit", request.PageSize));
            dataCmd.Parameters.Add(new MySqlParameter("@offset", offset));
            if (request.CategoryId.HasValue)
                dataCmd.Parameters.Add(new MySqlParameter("@categoryId", request.CategoryId.Value));

            await using var reader = await dataCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var source = reader.GetByte(reader.GetOrdinal("Source"));
                var thumbnailUrl = reader.IsDBNull(reader.GetOrdinal("ThumbnailUrl"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ThumbnailUrl"));
                var imageStoragePath = reader.IsDBNull(reader.GetOrdinal("ImageStoragePath"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ImageStoragePath"));
                var imageStorageProvider = reader.IsDBNull(reader.GetOrdinal("ImageStorageProvider"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ImageStorageProvider"));
                
                string? resolvedImageUrl = source == 3
                    ? thumbnailUrl
                    : (imageStoragePath != null ? _storage.BuildUrl(imageStoragePath) : null);

                items.Add(new FoodSearchResponse
                {
                    Id = Guid.Parse(reader.GetString(reader.GetOrdinal("Id"))),
                    NameVi = reader.GetString(reader.GetOrdinal("NameVi")),
                    NameEn = reader.IsDBNull(reader.GetOrdinal("NameEn"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("NameEn")),
                    CategoryId = reader.GetByte(reader.GetOrdinal("CategoryId")),
                    Source = source,
                    ServingSizeG = reader.GetDecimal(reader.GetOrdinal("ServingSizeG")),
                    ServingUnitVi = reader.IsDBNull(reader.GetOrdinal("ServingUnitVi"))
                        ? "g"
                        : reader.GetString(reader.GetOrdinal("ServingUnitVi")),
                    CaloriesKcal = reader.IsDBNull(reader.GetOrdinal("CaloriesKcal"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("CaloriesKcal")),
                    ImageUrl = resolvedImageUrl
                });
            }
        }

        return new PaginatedResponse<FoodSearchResponse>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Get all approved food items without fulltext search — used by the user-side "explore" tab.
    /// Uses EF Core LINQ to avoid raw SQL column name issues. Returns paginated results ordered by NameVi.
    /// </summary>
    public async Task<PaginatedResponse<FoodSearchResponse>> GetAllAsync(int page, int pageSize, byte? categoryId)
    {
        var query = _db.FoodItems
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .Where(f => f.Status == 1);

        if (categoryId.HasValue)
            query = query.Where(f => f.CategoryId == categoryId.Value);

        var totalCount = await query.CountAsync();

        var foods = await query
            .OrderBy(f => f.NameVi)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = foods.Select(f =>
        {
            string? resolvedImageUrl = f.Source == 3
                ? f.ThumbnailUrl
                : (f.ActiveImage?.StoragePath != null ? _storage.BuildUrl(f.ActiveImage.StoragePath) : null);

            return new FoodSearchResponse
            {
                Id = f.Id,
                NameVi = f.NameVi,
                NameEn = f.NameEn,
                CategoryId = f.CategoryId,
                Source = f.Source,
                ServingSizeG = f.ServingSizeG,
                ServingUnitVi = f.ServingUnitVi ?? "g",
                CaloriesKcal = f.Nutrition?.CaloriesKcal,
                ImageUrl = resolvedImageUrl
            };
        }).ToList();

        return new PaginatedResponse<FoodSearchResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Get food detail by ID with nutrition and resolved image URL.
    /// </summary>
    public async Task<FoodDetailResponse> GetByIdAsync(Guid id)
    {
        var food = await _db.FoodItems
            .Include(f => f.Category)
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (food == null)
            throw new NotFoundException("Không tìm thấy món ăn.");

        return _mapper.Map<FoodDetailResponse>(food);
    }

    /// <summary>
    /// Get components (children) of a parent food item.
    /// </summary>
    public async Task<List<FoodComponentResponse>> GetComponentsAsync(Guid foodItemId)
    {
        var exists = await _db.FoodItems.AnyAsync(f => f.Id == foodItemId);
        if (!exists)
            throw new NotFoundException("Không tìm thấy món ăn.");

        var components = await _db.FoodItemComponents
            .Where(c => c.ParentFoodId == foodItemId)
            .Include(c => c.ChildFood)
                .ThenInclude(cf => cf.Nutrition)
            .ToListAsync();

        return _mapper.Map<List<FoodComponentResponse>>(components);
    }

    /// <summary>
    /// Look up a food by barcode. Returns 404 if not found.
    /// </summary>
    public async Task<FoodDetailResponse> GetByBarcodeAsync(ulong barcode)
    {
        var food = await _db.FoodItems
            .Include(f => f.Category)
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .FirstOrDefaultAsync(f => f.Barcode == barcode && f.Status == 1);

        if (food == null)
            throw new NotFoundException("Không tìm thấy sản phẩm với mã vạch này.");

        return _mapper.Map<FoodDetailResponse>(food);
    }

    /// <summary>
    /// Create a community food item (source=3, status=0 pending).
    /// Applies Atwater validation: recalculates calories = P*4 + C*4 + F*9, overrides if delta > 2 kcal.
    /// </summary>
    public async Task<FoodDetailResponse> CreateAsync(CreateFoodRequest request, Guid userId)
    {
        // Validate category exists
        var categoryExists = await _db.FoodCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new BusinessException("INVALID_CATEGORY", "Danh mục không tồn tại.");

        // Atwater validation
        var atwaterCalories = (request.Nutrition.ProteinG * 4m)
                            + (request.Nutrition.CarbsG * 4m)
                            + (request.Nutrition.FatG * 9m);

        var finalCalories = Math.Abs(atwaterCalories - request.Nutrition.CaloriesKcal) > 2m
            ? atwaterCalories
            : request.Nutrition.CaloriesKcal;

        var foodItem = new FoodItem
        {
            Id = Guid.NewGuid(),
            NameVi = request.NameVi,
            NameEn = request.NameEn,
            CategoryId = request.CategoryId,
            Source = 3, // community
            Status = 0, // pending
            ServingSizeG = request.ServingSizeG,
            ServingUnitVi = request.ServingUnitVi,
            Barcode = request.Barcode,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            // Community items: do NOT set ActiveImageId
        };

        // Upload ảnh lên Cloudinary nếu có, lưu public_id vào ThumbnailUrl
        if (request.Image != null)
        {
            var publicId = await _storage.UploadAsync(request.Image, folder: "wao/foods");
            foodItem.ThumbnailUrl = _storage.BuildUrl(publicId);
        }

        var nutrition = new FoodNutrition
        {
            FoodItemId = foodItem.Id,
            CaloriesKcal = finalCalories,
            ProteinG = request.Nutrition.ProteinG,
            CarbsG = request.Nutrition.CarbsG,
            FatG = request.Nutrition.FatG,
            FiberG = request.Nutrition.FiberG,
            SugarG = request.Nutrition.SugarG,
            SodiumMg = request.Nutrition.SodiumMg,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.FoodItems.Add(foodItem);
        _db.FoodNutritions.Add(nutrition);
        await _db.SaveChangesAsync();

        // Reload with category for response
        await _db.Entry(foodItem).Reference(f => f.Category).LoadAsync();
        foodItem.Nutrition = nutrition;

        return _mapper.Map<FoodDetailResponse>(foodItem);
    }

    /// <summary>
    /// Get all meal types (lookup).
    /// </summary>
    public async Task<List<MealTypeResponse>> GetMealTypesAsync()
    {
        var mealTypes = await _db.MealTypes.ToListAsync();
        return _mapper.Map<List<MealTypeResponse>>(mealTypes);
    }
    
}
