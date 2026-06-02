namespace nutrition_app_backend.DTOs.Auth;

/// <summary>
/// Request body cho endpoint đăng nhập Google.
/// - Native (iOS/Android): gửi IdToken, Platform = "native"
/// - Web: gửi AccessToken, Platform = "web"
/// </summary>
public record GoogleLoginRequest(
    string? IdToken,
    string? AccessToken,
    string Platform  // "native" | "web"
);
