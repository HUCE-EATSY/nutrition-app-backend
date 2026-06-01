namespace nutrition_app_backend.Services.Storage;

public interface IStorageService
{
    /// <summary>
    /// Upload file lên cloud storage, trả về public_id để lưu vào DB.
    /// </summary>
    Task<string> UploadAsync(IFormFile file, string folder = "foods");

    /// <summary>
    /// Upload file từ remote URL lên cloud storage, trả về public_id để lưu vào DB.
    /// </summary>
    Task<string> UploadUrlAsync(string url, string folder = "foods");

    /// <summary>
    /// Chuyển public_id thành full public URL để trả cho client.
    /// Đây là nơi duy nhất biết format URL của từng provider.
    /// </summary>
    string BuildUrl(string publicId);
}
