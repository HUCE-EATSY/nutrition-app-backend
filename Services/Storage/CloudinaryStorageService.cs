using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace nutrition_app_backend.Services.Storage;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly string _cloudName;

    public CloudinaryStorageService(IConfiguration config)
    {
        _cloudName = config["Cloudinary:CloudName"]
            ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured.");

        var account = new Account(
            _cloudName,
            config["Cloudinary:ApiKey"]
                ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured."),
            config["Cloudinary:ApiSecret"]
                ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured.")
        );

        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    /// <summary>
    /// Upload IFormFile lên Cloudinary, trả về public_id (VD: "foods/abc123").
    /// public_id được lưu vào DB — không lưu full URL để dễ đổi provider sau.
    /// </summary>
    public async Task<string> UploadAsync(IFormFile file, string folder = "foods")
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            UseFilename = false,       // Dùng random ID do Cloudinary generate
            UniqueFilename = true,
            Overwrite = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.PublicId; // VD: "foods/xk8q3abc"
    }

    /// <summary>
    /// Upload file từ remote URL lên Cloudinary, trả về public_id.
    /// </summary>
    public async Task<string> UploadUrlAsync(string url, string folder = "foods")
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(url),
            Folder = folder,
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.PublicId;
    }

    /// <summary>
    /// Build full HTTPS URL từ public_id.
    /// VD: "foods/xk8q3abc" → "https://res.cloudinary.com/{cloud}/image/upload/foods/xk8q3abc"
    /// Đổi sang provider khác chỉ cần override method này.
    /// </summary>
    public string BuildUrl(string publicId)
        => $"https://res.cloudinary.com/{_cloudName}/image/upload/{publicId}";
}
