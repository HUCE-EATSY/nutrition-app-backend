using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Foods;
using nutrition_app_backend.Services.Storage;
using nutrition_app_backend.Services.OpenFoodFacts;
using nutrition_app_backend.Services.Spoonacular;
using AutoMapper;
using nutrition_app_backend.Enums;

namespace nutrition_app_backend.Services.Food;

public class FoodService : IFoodService
{
    private readonly WaoDbContext _db;
    private readonly IMapper _mapper;
    private readonly IStorageService _storage;
    private readonly IOpenFoodFactsService _offService;
    private readonly ISpoonacularService _spoonacular;

    public FoodService(WaoDbContext db, IMapper mapper,
                       IStorageService storage,
                       IOpenFoodFactsService offService,
                       ISpoonacularService spoonacular)
    {
        _db          = db;
        _mapper      = mapper;
        _storage     = storage;
        _offService  = offService;
        _spoonacular = spoonacular;
    }

    /// <summary>
    /// Delegates nutrition estimation from an image to SpoonacularService.
    /// Returns null if the food cannot be identified.
    /// </summary>
    public async Task<EstimatedFoodResponse?> EstimateNutrientsFromImageAsync(IFormFile image)
    {
        var publicId = await _storage.UploadAsync(image, folder: "wao/detections");
        var imageUrl = _storage.BuildUrl(publicId);
        return await _spoonacular.EstimateNutrientsAsync(imageUrl);
    }

    /// <summary>
    /// Fulltext search using MySQL MATCH … AGAINST in BOOLEAN MODE.
    /// - Approved items (status=1) visible to everyone.
    /// - Pending items (status=0) visible only to the creator.
    /// </summary>
    private static string EncodeCursor(DateTime createdAt, Guid id)
    {
        var plainText = $"{createdAt.Ticks}_{id}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(bytes);
    }

    private static (DateTime createdAt, Guid id)? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor)) return null;
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var plainText = System.Text.Encoding.UTF8.GetString(bytes);
            var parts = plainText.Split('_');
            if (parts.Length == 2 && long.TryParse(parts[0], out long ticks) && Guid.TryParse(parts[1], out Guid id))
            {
                return (new DateTime(ticks, DateTimeKind.Utc), id);
            }
        }
        catch
        {
            // Ignore invalid cursor
        }
        return null;
    }

    /// <summary>
    /// Paginated list of foods with cursor pagination.
    /// - Approved items (status=1) visible to everyone.
    /// - Pending items (status=0) visible only to the creator.
    /// </summary>
    public async Task<CursorPaginatedResponse<FoodSearchResponse>> GetListAsync(FoodListRequest request, Guid? currentUserId)
    {
        var query = _db.FoodItems.AsQueryable();

        // Filter by Status and Creator (Approved items are visible to everyone, Pending items only to the creator)
        query = query.Where(fi => fi.Status == FoodStatus.Approved 
                               || (fi.Status == FoodStatus.Pending && fi.CreatedBy == currentUserId));

        if (request.CategoryId.HasValue)
        {
            query = query.Where(fi => fi.CategoryId == request.CategoryId.Value);
        }

        // Apply cursor filter
        var cursorData = DecodeCursor(request.Cursor);
        if (cursorData.HasValue)
        {
            var (lastCreatedAt, lastId) = cursorData.Value;
            var lastIdStr = lastId.ToString();
            
            query = query.Where(fi => fi.CreatedAt < lastCreatedAt 
                                   || (fi.CreatedAt == lastCreatedAt && fi.Id.ToString().CompareTo(lastIdStr) < 0));
        }

        // Order by CreatedAt DESC and Id DESC as tie-breaker
        query = query.OrderByDescending(fi => fi.CreatedAt)
                     .ThenByDescending(fi => fi.Id);

        // Fetch PageSize + 1 items to determine if there is a next page
        var foodItems = await query
            .Include(fi => fi.Nutrition)
            .Include(fi => fi.ActiveImage)
            .Take(request.PageSize + 1)
            .ToListAsync();

        var hasMore = foodItems.Count > request.PageSize;
        var itemsToReturn = foodItems.Take(request.PageSize).ToList();

        var items = itemsToReturn.Select(fi =>
        {
            string? resolvedImageUrl = fi.Source == FoodSource.Community
                ? fi.ThumbnailUrl
                : (fi.ActiveImage != null ? _storage.BuildUrl(fi.ActiveImage.StoragePath) : null);

            return new FoodSearchResponse
            {
                Id = fi.Id,
                NameVi = fi.NameVi,
                NameEn = fi.NameEn,
                CategoryId = fi.CategoryId,
                Source = (byte)fi.Source,
                ServingSizeG = fi.ServingSizeG,
                ServingUnitVi = fi.ServingUnitVi ?? "g",
                CaloriesKcal = fi.Nutrition?.CaloriesKcal,
                ProteinG = fi.Nutrition?.ProteinG,
                CarbsG = fi.Nutrition?.CarbsG,
                FatG = fi.Nutrition?.FatG,
                ImageUrl = resolvedImageUrl
            };
        }).ToList();

        string? nextCursor = null;
        if (hasMore && itemsToReturn.Any())
        {
            var lastItem = itemsToReturn.Last();
            nextCursor = EncodeCursor(lastItem.CreatedAt, lastItem.Id);
        }

        return new CursorPaginatedResponse<FoodSearchResponse>
        {
            Items = items,
            NextCursor = nextCursor,
            PageSize = request.PageSize
        };
    }

    public async Task<PaginatedResponse<FoodSearchResponse>> SearchAsync(FoodSearchRequest request, Guid? currentUserId)
    {
        var searchTerm = request.Q.Trim();
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new BusinessException("EMPTY_SEARCH", "Từ khóa tìm kiếm không được để trống.");

        var booleanTerm = string.Join(" ", searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => $"+{w}*"));

        var userIdStr = currentUserId?.ToString() ?? "";
        var offset = (request.Page - 1) * request.PageSize;

        var whereClause = $@"
            MATCH(fi.NameVi, fi.NameEn) AGAINST (@searchTerm IN BOOLEAN MODE)
            AND (fi.Status = {(byte)FoodStatus.Approved} OR (fi.Status = {(byte)FoodStatus.Pending} AND fi.CreatedBy = @userId))";

        if (request.CategoryId.HasValue)
        {
            whereClause += " AND fi.CategoryId = @categoryId";
        }

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

        var dataSql = $@"
            SELECT
                fi.Id                AS Id,
                fi.NameVi           AS NameVi,
                fi.NameEn           AS NameEn,
                fi.CategoryId       AS CategoryId,
                fi.Source            AS Source,
                fi.ServingSizeG    AS ServingSizeG,
                fi.ServingUnitVi   AS ServingUnitVi,
                fi.ThumbnailUrl     AS ThumbnailUrl,
                fi.ActiveImageId   AS ActiveImageId,
                fn.CaloriesKcal     AS CaloriesKcal,
                fn.ProteinG         AS ProteinG,
                fn.CarbsG           AS CarbsG,
                fn.FatG             AS FatG,
                fii.StoragePath     AS ImageStoragePath,
                fii.StorageProvider AS ImageStorageProvider
            FROM food_items fi
            LEFT JOIN food_nutrition fn ON fn.FoodItemId = fi.Id
            LEFT JOIN food_item_images fii ON fi.Source != {(byte)FoodSource.Community} AND fii.Id = fi.ActiveImageId
            WHERE {whereClause}
            ORDER BY MATCH(fi.NameVi, fi.NameEn) AGAINST (@searchTerm2 IN BOOLEAN MODE) DESC
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
                
                string? resolvedImageUrl = source == (byte)FoodSource.Community
                    ? thumbnailUrl
                    : (imageStoragePath != null ? _storage.BuildUrl(imageStoragePath) : null);

                items.Add(new FoodSearchResponse
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
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
                    ProteinG = reader.IsDBNull(reader.GetOrdinal("ProteinG"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("ProteinG")),
                    CarbsG = reader.IsDBNull(reader.GetOrdinal("CarbsG"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("CarbsG")),
                    FatG = reader.IsDBNull(reader.GetOrdinal("FatG"))
                        ? null
                        : reader.GetDecimal(reader.GetOrdinal("FatG")),
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
            .Include(c => c.ChildFood)
                .ThenInclude(cf => cf.ActiveImage)
            .ToListAsync();

        return _mapper.Map<List<FoodComponentResponse>>(components);
    }

    /// <summary>
    /// Tra cứu sản phẩm theo mã vạch với Cache-Aside Pattern:
    /// 1. Cache Hit  — Trả về từ local DB ngay lập tức.
    /// 2. Cache Miss — Gọi Open Food Facts API, map + lưu vào DB, rồi trả về.
    /// 3. Not Found  — Trả về null nếu cả hai nguồn đều không có.
    /// </summary>
    public async Task<FoodDetailResponse?> GetByBarcodeAsync(string barcode)
    {
        // Validate: barcode không được rỗng
        if (string.IsNullOrWhiteSpace(barcode))
            return null;

        var cleanBarcode = barcode.Trim();

        // STEP 1 — Cache Hit: query local DB
        var food = await _db.FoodItems
            .Include(f => f.Category)
            .Include(f => f.Nutrition)
            .Include(f => f.ActiveImage)
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Barcode == cleanBarcode
                                   && f.Status == FoodStatus.Approved);

        if (food != null)
            return _mapper.Map<FoodDetailResponse>(food);

        // STEP 2 — Cache Miss: gọi Open Food Facts API
        var offProduct = await _offService.LookupByBarcodeAsync(barcode);

        if (offProduct == null)
            return null; // Controller sẽ trả 404 + canContribute: true

        // STEP 3 — Map OFF → FoodItem entity
        var nutriments = offProduct.Product!.Nutriments;
        var newFood = new FoodItem
        {
            Id            = Guid.NewGuid(),
            NameVi        = offProduct.Product.ProductName ?? "Sản phẩm chưa có tên",
            NameEn        = offProduct.Product.ProductNameEn,
            Barcode       = cleanBarcode,
            Source        = FoodSource.OpenFoodFacts,
            Status        = FoodStatus.Approved,
            CategoryId    = 10,             // fallback: "Khác" (Other)
            ServingSizeG  = 100m,
            ServingUnitVi = "g",
            ThumbnailUrl  = offProduct.Product.ImageUrl,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };

        var newNutrition = new FoodNutrition
        {
            FoodItemId   = newFood.Id,
            CaloriesKcal = (decimal)(nutriments?.EnergyKcal100g ?? 0),
            ProteinG     = (decimal)(nutriments?.Proteins100g   ?? 0),
            CarbsG       = (decimal)(nutriments?.Carbs100g      ?? 0),
            FatG         = (decimal)(nutriments?.Fat100g        ?? 0),
            FiberG       = nutriments?.Fiber100g  is not null ? (decimal)nutriments.Fiber100g  : null,
            SugarG       = nutriments?.Sugars100g is not null ? (decimal)nutriments.Sugars100g : null,
            // OFF sodium là g/100g → chuyển sang mg
            SodiumMg     = nutriments?.Sodium100g is not null ? (decimal)nutriments.Sodium100g * 1000m : null,
            UpdatedAt    = DateTime.UtcNow,
        };

        // STEP 4 — Save to DB
        _db.FoodItems.Add(newFood);
        _db.FoodNutritions.Add(newNutrition);
        await _db.SaveChangesAsync();

        // Gán Nutrition để mapper có đủ dữ liệu mà không cần reload
        newFood.Nutrition = newNutrition;
        return _mapper.Map<FoodDetailResponse>(newFood);
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
            Source = FoodSource.Community, // community
            Status = FoodStatus.Pending, // pending
            ServingSizeG = request.ServingSizeG,
            ServingUnitVi = request.ServingUnitVi ?? "g",
            Barcode = request.Barcode,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            // Community items: do NOT set ActiveImageId
        };

        // Upload ảnh lên Cloudinary nếu có, lưu public_id vào ThumbnailUrl.
        // Nếu frontend đã upload trước (flow nhận diện AI), dùng ImageUrl trực tiếp.
        if (request.Image != null)
        {
            var publicId = await _storage.UploadAsync(request.Image, folder: "wao/foods");
            foodItem.ThumbnailUrl = _storage.BuildUrl(publicId);
        }
        else if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            foodItem.ThumbnailUrl = request.ImageUrl;
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
    /// Create a custom recipe (composite food) for a user.
    /// Automatically calculates nutrition based on selected ingredients.
    /// </summary>
    public async Task<FoodDetailResponse> CreateRecipeAsync(CreateRecipeRequest request, Guid userId)
    {
        // Validate category
        var categoryExists = await _db.FoodCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new BusinessException("INVALID_CATEGORY", "Danh mục không tồn tại.");

        if (request.Components == null || request.Components.Count == 0)
            throw new BusinessException("EMPTY_COMPONENTS", "Công thức phải có ít nhất 1 thành phần.");

        var childFoodIds = request.Components.Select(c => c.ChildFoodId).ToList();
        var childFoods = await _db.FoodItems
            .Include(f => f.Nutrition)
            .Where(f => childFoodIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id);

        foreach (var comp in request.Components)
        {
            if (!childFoods.ContainsKey(comp.ChildFoodId))
                throw new NotFoundException($"Không tìm thấy nguyên liệu với ID: {comp.ChildFoodId}");
        }

        // Calculate totals
        decimal totalCalories = 0;
        decimal totalProtein = 0;
        decimal totalCarbs = 0;
        decimal totalFat = 0;
        decimal totalFiber = 0;
        decimal totalSugar = 0;
        decimal totalSodium = 0;
        decimal totalWeightG = 0;

        foreach (var comp in request.Components)
        {
            var childFood = childFoods[comp.ChildFoodId];
            if (childFood.ServingSizeG <= 0) continue; // Safety check

            var ratio = comp.QuantityG / childFood.ServingSizeG;

            totalWeightG += comp.QuantityG;

            if (childFood.Nutrition != null)
            {
                totalCalories += childFood.Nutrition.CaloriesKcal * ratio;
                totalProtein += childFood.Nutrition.ProteinG * ratio;
                totalCarbs += childFood.Nutrition.CarbsG * ratio;
                totalFat += childFood.Nutrition.FatG * ratio;
                totalFiber += (childFood.Nutrition.FiberG ?? 0) * ratio;
                totalSugar += (childFood.Nutrition.SugarG ?? 0) * ratio;
                totalSodium += (childFood.Nutrition.SodiumMg ?? 0) * ratio;
            }
        }

        var recipeItem = new FoodItem
        {
            Id = Guid.NewGuid(),
            NameVi = request.NameVi,
            NameEn = request.NameEn,
            CategoryId = request.CategoryId,
            Source = FoodSource.Community, // Community/User
            Status = FoodStatus.Pending, // Pending/Private
            ServingSizeG = totalWeightG > 0 ? totalWeightG : 100, // Fallback if weight is 0
            ServingUnitVi = request.ServingUnitVi,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        if (request.Image != null)
        {
            var publicId = await _storage.UploadAsync(request.Image, folder: "wao/foods");
            recipeItem.ThumbnailUrl = _storage.BuildUrl(publicId);
        }

        var recipeNutrition = new FoodNutrition
        {
            FoodItemId = recipeItem.Id,
            CaloriesKcal = totalCalories,
            ProteinG = totalProtein,
            CarbsG = totalCarbs,
            FatG = totalFat,
            FiberG = totalFiber,
            SugarG = totalSugar,
            SodiumMg = totalSodium,
            UpdatedAt = DateTime.UtcNow,
        };

        var components = request.Components.Select(c => new FoodItemComponent
        {
            ParentFoodId = recipeItem.Id,
            ChildFoodId = c.ChildFoodId,
            QuantityG = c.QuantityG
        }).ToList();

        _db.FoodItems.Add(recipeItem);
        _db.FoodNutritions.Add(recipeNutrition);
        _db.FoodItemComponents.AddRange(components);
        
        await _db.SaveChangesAsync();

        // Reload for response
        await _db.Entry(recipeItem).Reference(f => f.Category).LoadAsync();
        recipeItem.Nutrition = recipeNutrition;

        return _mapper.Map<FoodDetailResponse>(recipeItem);
    }

    /// <summary>
    /// Get all meal types (lookup).
    /// </summary>
    public async Task<List<MealTypeResponse>> GetMealTypesAsync()
    {
        var mealTypes = await _db.MealTypes.ToListAsync();
        return _mapper.Map<List<MealTypeResponse>>(mealTypes);
    }

    /// <summary>
    /// Update a custom food item. Only the owner can update.
    /// Applies Atwater validation: recalculates calories = P*4 + C*4 + F*9, overrides if delta > 2 kcal.
    /// </summary>
    public async Task<FoodDetailResponse> UpdateAsync(Guid id, CreateFoodRequest request, Guid userId)
    {
        var foodItem = await _db.FoodItems
            .Include(f => f.Nutrition)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (foodItem == null)
            throw new NotFoundException("Không tìm thấy món ăn.");

        if (foodItem.CreatedBy != userId)
            throw new BusinessException("UNAUTHORIZED_ACCESS", "Bạn không có quyền sửa món ăn này.");

        if (foodItem.Source == FoodSource.Official)
            throw new BusinessException("RESTRICTED_ACTION", "Không thể sửa món ăn chính thức của hệ thống.");

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

        foodItem.NameVi = request.NameVi;
        foodItem.NameEn = request.NameEn;
        foodItem.CategoryId = request.CategoryId;
        foodItem.ServingSizeG = request.ServingSizeG;
        foodItem.ServingUnitVi = request.ServingUnitVi ?? "g";
        foodItem.Barcode = request.Barcode;
        foodItem.UpdatedAt = DateTime.UtcNow;

        if (request.Image != null)
        {
            var publicId = await _storage.UploadAsync(request.Image, folder: "wao/foods");
            foodItem.ThumbnailUrl = _storage.BuildUrl(publicId);
        }
        else if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            foodItem.ThumbnailUrl = request.ImageUrl;
        }

        if (foodItem.Nutrition == null)
        {
            foodItem.Nutrition = new FoodNutrition { FoodItemId = foodItem.Id };
            _db.FoodNutritions.Add(foodItem.Nutrition);
        }

        foodItem.Nutrition.CaloriesKcal = finalCalories;
        foodItem.Nutrition.ProteinG = request.Nutrition.ProteinG;
        foodItem.Nutrition.CarbsG = request.Nutrition.CarbsG;
        foodItem.Nutrition.FatG = request.Nutrition.FatG;
        foodItem.Nutrition.FiberG = request.Nutrition.FiberG;
        foodItem.Nutrition.SugarG = request.Nutrition.SugarG;
        foodItem.Nutrition.SodiumMg = request.Nutrition.SodiumMg;
        foodItem.Nutrition.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // Reload with category
        await _db.Entry(foodItem).Reference(f => f.Category).LoadAsync();

        return _mapper.Map<FoodDetailResponse>(foodItem);
    }

    /// <summary>
    /// Update a custom recipe. Only the owner can update.
    /// Automatically recalculates nutrition based on selected ingredients.
    /// </summary>
    public async Task<FoodDetailResponse> UpdateRecipeAsync(Guid id, CreateRecipeRequest request, Guid userId)
    {
        var recipeItem = await _db.FoodItems
            .Include(f => f.Nutrition)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (recipeItem == null)
            throw new NotFoundException("Không tìm thấy công thức.");

        if (recipeItem.CreatedBy != userId)
            throw new BusinessException("UNAUTHORIZED_ACCESS", "Bạn không có quyền sửa công thức này.");

        if (recipeItem.Source == FoodSource.Official)
            throw new BusinessException("RESTRICTED_ACTION", "Không thể sửa công thức chính thức của hệ thống.");

        // Validate category
        var categoryExists = await _db.FoodCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new BusinessException("INVALID_CATEGORY", "Danh mục không tồn tại.");

        if (request.Components == null || request.Components.Count == 0)
            throw new BusinessException("EMPTY_COMPONENTS", "Công thức phải có ít nhất 1 thành phần.");

        var childFoodIds = request.Components.Select(c => c.ChildFoodId).ToList();
        var childFoods = await _db.FoodItems
            .Include(f => f.Nutrition)
            .Where(f => childFoodIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id);

        foreach (var comp in request.Components)
        {
            if (!childFoods.ContainsKey(comp.ChildFoodId))
                throw new NotFoundException($"Không tìm thấy nguyên liệu với ID: {comp.ChildFoodId}");
        }

        // Calculate totals
        decimal totalCalories = 0;
        decimal totalProtein = 0;
        decimal totalCarbs = 0;
        decimal totalFat = 0;
        decimal totalFiber = 0;
        decimal totalSugar = 0;
        decimal totalSodium = 0;
        decimal totalWeightG = 0;

        foreach (var comp in request.Components)
        {
            var childFood = childFoods[comp.ChildFoodId];
            if (childFood.ServingSizeG <= 0) continue;

            var ratio = comp.QuantityG / childFood.ServingSizeG;
            totalWeightG += comp.QuantityG;

            if (childFood.Nutrition != null)
            {
                totalCalories += childFood.Nutrition.CaloriesKcal * ratio;
                totalProtein += childFood.Nutrition.ProteinG * ratio;
                totalCarbs += childFood.Nutrition.CarbsG * ratio;
                totalFat += childFood.Nutrition.FatG * ratio;
                totalFiber += (childFood.Nutrition.FiberG ?? 0) * ratio;
                totalSugar += (childFood.Nutrition.SugarG ?? 0) * ratio;
                totalSodium += (childFood.Nutrition.SodiumMg ?? 0) * ratio;
            }
        }

        recipeItem.NameVi = request.NameVi;
        recipeItem.NameEn = request.NameEn;
        recipeItem.CategoryId = request.CategoryId;
        recipeItem.ServingSizeG = totalWeightG > 0 ? totalWeightG : 100;
        recipeItem.ServingUnitVi = request.ServingUnitVi;
        recipeItem.UpdatedAt = DateTime.UtcNow;

        if (request.Image != null)
        {
            var publicId = await _storage.UploadAsync(request.Image, folder: "wao/foods");
            recipeItem.ThumbnailUrl = _storage.BuildUrl(publicId);
        }

        if (recipeItem.Nutrition == null)
        {
            recipeItem.Nutrition = new FoodNutrition { FoodItemId = recipeItem.Id };
            _db.FoodNutritions.Add(recipeItem.Nutrition);
        }

        recipeItem.Nutrition.CaloriesKcal = totalCalories;
        recipeItem.Nutrition.ProteinG = totalProtein;
        recipeItem.Nutrition.CarbsG = totalCarbs;
        recipeItem.Nutrition.FatG = totalFat;
        recipeItem.Nutrition.FiberG = totalFiber;
        recipeItem.Nutrition.SugarG = totalSugar;
        recipeItem.Nutrition.SodiumMg = totalSodium;
        recipeItem.Nutrition.UpdatedAt = DateTime.UtcNow;

        // Update components: delete old ones first, then add new ones
        var oldComponents = await _db.FoodItemComponents
            .Where(c => c.ParentFoodId == recipeItem.Id)
            .ToListAsync();
        _db.FoodItemComponents.RemoveRange(oldComponents);

        var newComponents = request.Components.Select(c => new FoodItemComponent
        {
            ParentFoodId = recipeItem.Id,
            ChildFoodId = c.ChildFoodId,
            QuantityG = c.QuantityG
        }).ToList();
        _db.FoodItemComponents.AddRange(newComponents);

        await _db.SaveChangesAsync();

        // Reload with category
        await _db.Entry(recipeItem).Reference(f => f.Category).LoadAsync();

        return _mapper.Map<FoodDetailResponse>(recipeItem);
    }

    /// <summary>
    /// Delete a custom food item or recipe. Only the owner can delete, and only if not logged in food logs.
    /// </summary>
    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var foodItem = await _db.FoodItems.FirstOrDefaultAsync(f => f.Id == id);
        if (foodItem == null)
            throw new NotFoundException("Không tìm thấy món ăn hoặc công thức.");

        if (foodItem.CreatedBy != userId)
            throw new BusinessException("UNAUTHORIZED_ACCESS", "Bạn không có quyền xóa mục này.");

        if (foodItem.Source == FoodSource.Official)
            throw new BusinessException("RESTRICTED_ACTION", "Không thể xóa mục chính thức của hệ thống.");

        // Check if referenced in user food logs
        var isLogged = await _db.FoodLogs.AnyAsync(l => l.FoodItemId == id);
        if (isLogged)
            throw new BusinessException("FOOD_IN_USE", "Không thể xóa mục này vì nó đã được sử dụng trong nhật ký ăn uống.");

        _db.FoodItems.Remove(foodItem);
        await _db.SaveChangesAsync();
    }
}
