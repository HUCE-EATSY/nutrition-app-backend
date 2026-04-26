using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Auth;
using nutrition_app_backend.Models.Users;


namespace nutrition_app_backend.Services.Auth;

public class AuthService : IAuthService
{
    private readonly WaoDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(WaoDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginWithGoogleAsync(GoogleLoginRequest request)
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
            // 1. Dùng thư viện Google SDK để giải mã và xác thực IdToken từ React Native
            // Nó sẽ tự động check chữ ký điện tử và hạn sử dụng của token với máy chủ Google
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
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
            user = authProvider.User; // Đã từng đăng nhập
        }
        else
        {
            // 3. User mới tinh -> Tạo dữ liệu vào DB
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
                VerifiedAt = DateTime.UtcNow // Google đã verify email cho mình rồi
            };
            _dbContext.UserAuthProviders.Add(newAuth);

            await _dbContext.SaveChangesAsync();
        }

        // 4. Sinh JWT của ứng dụng Wao
        string accessToken = GenerateJwtToken(user.Id, email);

        return new AuthResponse(user.Id, email, accessToken, isNewUser);
    }
    private string GenerateJwtToken(Guid userId, string email)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(2), 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}