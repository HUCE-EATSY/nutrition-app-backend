using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Auth;
using nutrition_app_backend.Exceptions;
using nutrition_app_backend.Models.Users;
using nutrition_app_backend.Services.Token;


namespace nutrition_app_backend.Services.Auth;

public class AuthService : IAuthService
{
    private readonly WaoDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthService(WaoDbContext dbContext, IConfiguration configuration, ITokenService tokenService, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenService = tokenService;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
    {
        string providerUid;
        string email;

        if (request.Platform == "web")
        {
            // Web: dùng access_token gọi Google UserInfo API để lấy thông tin user
            (providerUid, email) = await GetUserInfoFromAccessTokenAsync(request.AccessToken
                ?? throw new BusinessException("MISSING_TOKEN", "AccessToken is required for web login."));
        }
        else
        {
            // Native (iOS/Android): xác thực idToken bằng GoogleJsonWebSignature
            (providerUid, email) = await ValidateIdTokenAsync(request.IdToken
                ?? throw new BusinessException("MISSING_TOKEN", "IdToken is required for native login."));
        }

        // 2. Tìm User trong Database
        var authProvider = await _dbContext.UserAuthProviders
            .Include(a => a.User) // Join bảng để lấy luôn cục User
            .FirstOrDefaultAsync(a => a.Provider == "google" && a.ProviderUid == providerUid);

        Models.Users.User user;
        bool isNewUser = false;

        if (authProvider != null && !authProvider.User.DeletedAt.HasValue)
        {
            // Tài khoản bình thường — đăng nhập bình thường
            user = authProvider.User;
        }
        else if (authProvider != null && authProvider.User.DeletedAt.HasValue)
        {
            // Tài khoản đã bị xóa trước đó — coi như đăng ký lại:
            // Tạo User mới hoàn toàn sạch, trỏ lại AuthProvider về userId mới.
            isNewUser = true;
            user = new Models.Users.User();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            authProvider.UserId = user.Id;
        }
        else
        {
            // Lần đầu đăng nhập — tạo User + AuthProvider mới
            isNewUser = true;
            user = new Models.Users.User();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var newAuth = new UserAuthProvider
            {
                UserId = user.Id,
                Provider = "google",
                ProviderUid = providerUid,
                Email = email,
                VerifiedAt = DateTime.UtcNow
            };
            _dbContext.UserAuthProviders.Add(newAuth);
        }

        await _dbContext.SaveChangesAsync();
        return await _tokenService.CreateTokensAsync(user, isNewUser, email);
    }

    // ----- Private helpers -----

    /// <summary>
    /// Xác thực idToken từ Native (iOS/Android) bằng thư viện Google.Apis.Auth.
    /// </summary>
    private async Task<(string providerUid, string email)> ValidateIdTokenAsync(string idToken)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
                    _configuration["Google:WebClientId"],
                    _configuration["Google:IosClientId"],
                    _configuration["Google:AndroidClientId"]
                }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
        }
        catch (Exception ex)
        {
            throw new BusinessException("INVALID_GOOGLE_TOKEN", "Invalid Google IdToken.", ex);
        }

        return (payload.Subject, payload.Email);
    }

    /// <summary>
    /// Lấy thông tin user từ access_token của Web bằng cách gọi Google UserInfo API.
    /// Bảo mật tương đương — backend vẫn là bên xác thực và cấp JWT.
    /// </summary>
    private async Task<(string providerUid, string email)> GetUserInfoFromAccessTokenAsync(string accessToken)
    {
        var response = await _httpClient.GetAsync(
            $"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");

        if (!response.IsSuccessStatusCode)
        {
            throw new BusinessException("INVALID_GOOGLE_TOKEN", "Could not retrieve user info from Google access token.");
        }

        var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>()
            ?? throw new BusinessException("INVALID_GOOGLE_TOKEN", "Empty user info response from Google.");

        if (string.IsNullOrEmpty(userInfo.Sub) || string.IsNullOrEmpty(userInfo.Email))
        {
            throw new BusinessException("INVALID_GOOGLE_TOKEN", "Google user info is missing required fields.");
        }

        return (userInfo.Sub, userInfo.Email);
    }

    /// <summary>
    /// POCO để deserialize response từ Google UserInfo API (v3).
    /// </summary>
    private sealed class GoogleUserInfo
    {
        public string Sub { get; init; } = string.Empty;   // providerUid duy nhất
        public string Email { get; init; } = string.Empty;
    }
}
