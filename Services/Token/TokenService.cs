namespace nutrition_app_backend.Services.Token;

using nutrition_app_backend.Data;
using nutrition_app_backend.DTOs.Auth;
using nutrition_app_backend.Models.Users;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly WaoDbContext _dbContext;
    private readonly IConfiguration _config;

    public TokenService(WaoDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    // ================= CREATE TOKENS =================

    public async Task<AuthResponse> CreateTokensAsync(User user, bool isNewUser, string email)
    {
        var accessToken = GenerateAccessToken(user.Id, email);
        var refreshToken = await CreateRefreshTokenAsync(user);

        return new AuthResponse(
            user.Id,
            email,
            accessToken,
            refreshToken,
            isNewUser
        );
    }

    // ================= REFRESH =================

    public async Task<AuthResponse?> RefreshAsync(string refreshToken)
    {
        var hash = HashToken(refreshToken);

        var stored = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.HashedToken == hash);

        if (stored == null || !stored.IsValid)
            return null;

        // revoke old token
        stored.RevokedAt = DateTime.UtcNow;

        // create new refresh token
        var newRefreshToken = GenerateRefreshToken();
        var newHash = HashToken(newRefreshToken);

        var newEntity = new RefreshToken
        {
            UserId = stored.UserId,
            HashedToken = newHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!)
            )
        };

        _dbContext.RefreshTokens.Add(newEntity);

        // ⚡ IMPORTANT: email comes from UserAuthProvider OR stored snapshot logic
        // Since we removed dependency, we should NOT use navigation here.
        // Better: store email in RefreshToken OR reload minimal data if needed.

        var email = await GetEmailByUserId(stored.UserId);

        var newAccessToken = GenerateAccessToken(stored.UserId, email);

        await _dbContext.SaveChangesAsync();

        return new AuthResponse(
            stored.UserId,
            email,
            newAccessToken,
            newRefreshToken,
            false
        );
    }

    // ================= REVOKE =================

    public async Task RevokeAsync(string refreshToken)
    {
        var hash = HashToken(refreshToken);

        var stored = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.HashedToken == hash);

        if (stored == null || !stored.IsValid) 
            return;

        stored.RevokedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    // ================= CREATE REFRESH TOKEN =================

    private async Task<string> CreateRefreshTokenAsync(User user)
    {
        var raw = GenerateRefreshToken();
        var hash = HashToken(raw);

        var entity = new RefreshToken
        {
            UserId = user.Id,
            HashedToken = hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!)
            )
        };

        _dbContext.RefreshTokens.Add(entity);
        await _dbContext.SaveChangesAsync();
        return raw;
    }

    // ================= JWT =================

    private string GenerateAccessToken(Guid userId, string email)
    {
        var key = _config["Jwt:Key"]!;
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpirationMinutes"]!)
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ================= EMAIL RESOLVER (SAFE VERSION) =================

    private async Task<string> GetEmailByUserId(Guid userId)
    {
        var email = await _dbContext.UserAuthProviders
            .Where(x => x.UserId == userId)
            .Select(x => x.Email)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(email))
            throw new Exception("Email not found for user");

        return email;
    }

    // ================= HELPERS =================

    private string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(
            sha256.ComputeHash(Encoding.UTF8.GetBytes(token))
        );
    }
}