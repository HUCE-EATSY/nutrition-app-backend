namespace nutrition_app_backend.DTOs.Auth;

public record AuthResponse(
    Guid UserId, 
    string Email, 
    string AccessToken, 
    bool IsNewUser
);