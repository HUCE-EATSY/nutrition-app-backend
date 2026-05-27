using nutrition_app_backend.DTOs.OpenFoodFacts;

namespace nutrition_app_backend.Services.OpenFoodFacts;

public interface IOpenFoodFactsService
{
    /// <summary>
    /// Tra cứu thông tin sản phẩm từ Open Food Facts API qua mã vạch.
    /// </summary>
    /// <param name="barcode">Chuỗi mã vạch (EAN-13 hoặc UPC-A)</param>
    /// <returns>Trả về DTO chứa thông tin nếu tìm thấy, ngược lại trả về null</returns>
    Task<OffProductDto?> LookupByBarcodeAsync(string barcode);
}
