using nutrition_app_backend.DTOs.Auth;

namespace nutrition_app_backend.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse?> LoginWithGoogleAsync(GoogleLoginRequest request);
}