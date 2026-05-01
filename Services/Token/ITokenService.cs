namespace nutrition_app_backend.Services.Token;
using nutrition_app_backend.DTOs.Auth;
using Models.Users;

public interface ITokenService
{
    Task<AuthResponse> CreateTokensAsync(User user, bool isNewUser, string email);
    Task<AuthResponse?> RefreshAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}