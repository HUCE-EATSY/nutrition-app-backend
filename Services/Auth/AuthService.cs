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

    public AuthService(WaoDbContext dbContext, IConfiguration configuration, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
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
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
        }
        catch (Exception ex)
        {
            throw new BusinessException("INVALID_GOOGLE_TOKEN", "Invalid Google token.", ex);
        }

        string providerUid = payload.Subject; // Mã định danh duy nhất của user từ Google
        string email = payload.Email;

        // 2. Tìm User trong Database
        var authProvider = await _dbContext.UserAuthProviders
            .Include(a => a.User) // Join bảng để lấy luôn cục User
            .FirstOrDefaultAsync(a => a.Provider == "google" && a.ProviderUid == providerUid);

        Models.Users.User user;
        bool isNewUser = false;

        if (authProvider != null)
        {
            user = authProvider.User;
        }
        else
        {
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

            await _dbContext.SaveChangesAsync();
        }
        return await _tokenService.CreateTokensAsync(user, isNewUser, email);
    }
}
