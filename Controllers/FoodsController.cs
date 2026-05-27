using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nutrition_app_backend.DTOs;
using nutrition_app_backend.DTOs.Foods;
using nutrition_app_backend.Extensions;
using nutrition_app_backend.Services.Food;

namespace nutrition_app_backend.Controllers;

[ApiController]
[Route("api/foods")]
[Authorize]
public class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;

    public FoodsController(IFoodService foodService)
    {
        _foodService = foodService;
    }

    /// <summary>
    /// Get a paginated list of foods (for default display, no search).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<FoodSearchResponse>>>> GetList([FromQuery] FoodListRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.GetListAsync(request, userId);

        return Ok(ApiResponse<PaginatedResponse<FoodSearchResponse>>.Success(result, "Lấy danh sách thành công"));
    }

    /// <summary>
    /// Fulltext search food items. Approved items visible to all; pending items only to creator.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<FoodSearchResponse>>>> Search([FromQuery] FoodSearchRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.SearchAsync(request, userId);

        return Ok(ApiResponse<PaginatedResponse<FoodSearchResponse>>.Success(result, "Tìm kiếm thành công"));
    }

    /// <summary>
    /// Get food detail by ID with nutrition and image.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> GetById([FromRoute] Guid id)
    {
        var result = await _foodService.GetByIdAsync(id);

        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Lấy chi tiết món ăn thành công"));
    }

    /// <summary>
    /// Get components (children) of a food item.
    /// </summary>
    [HttpGet("{id:guid}/components")]
    public async Task<ActionResult<ApiResponse<List<FoodComponentResponse>>>> GetComponents([FromRoute] Guid id)
    {
        var result = await _foodService.GetComponentsAsync(id);

        return Ok(ApiResponse<List<FoodComponentResponse>>.Success(result, "Lấy thành phần thành công"));
    }

    /// <summary>
    /// Barcode lookup — Cache-Aside: Local DB trước, OFF API nếu không có.
    /// Returns 404 với canContribute: true nếu không tìm thấy ở đâu.
    /// </summary>
    [HttpGet("barcode/{barcode}")]
    [ProducesResponseType(typeof(ApiResponse<FoodDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> GetByBarcode([FromRoute] string barcode)
    {
        var result = await _foodService.GetByBarcodeAsync(barcode);

        if (result == null)
            return NotFound(ApiResponse<object>.Fail(
                "Không tìm thấy sản phẩm. Bạn có thể đóng góp thông tin.",
                "404",
                new { canContribute = true, barcode }
            ));

        return Ok(ApiResponse<FoodDetailResponse>.Success(result, "Tìm thấy sản phẩm"));
    }


    /// <summary>
    /// Estimate nutrition from a food image URL (Cloudinary .webp).
    /// Transforms the URL to .jpg, calls Spoonacular's estimateNutrients API,
    /// and returns a pre-filled response the frontend can display and submit.
    /// </summary>
    [HttpPost("estimate-nutrients")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<EstimatedFoodResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EstimatedFoodResponse>>> EstimateNutrients(
        [FromForm] EstimateNutrientsRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest(ApiResponse<object>.Fail("Không tìm thấy file ảnh.", "400"));

        var result = await _foodService.EstimateNutrientsFromImageAsync(request.Image);

        if (result == null)
            return NotFound(ApiResponse<object>.Fail(
                "Không thể nhận dạng thực phẩm từ ảnh này. Vui lòng thử ảnh khác.",
                "404"));

        return Ok(ApiResponse<EstimatedFoodResponse>.Success(result, "Phân tích dinh dưỡng thành công"));
    }

    /// <summary>
    /// Create a community food item (source=3, status=0 pending).
    /// Gửi dưới dạng multipart/form-data. Ảnh (image) sẽ được backend upload lên Cloudinary.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> Create([FromForm] CreateFoodRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.CreateAsync(request, userId);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<FoodDetailResponse>.Success(result, "Tạo món ăn thành công", "201"));
    }

    /// <summary>
    /// Create a custom recipe (composite food) from existing ingredients.
    /// Gửi dưới dạng multipart/form-data.
    /// </summary>
    [HttpPost("recipes")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<FoodDetailResponse>>> CreateRecipe([FromForm] CreateRecipeRequest request)
    {
        Guid userId = User.GetUserId();
        var result = await _foodService.CreateRecipeAsync(request, userId);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<FoodDetailResponse>.Success(result, "Tạo công thức thành công", "201"));
    }
}
