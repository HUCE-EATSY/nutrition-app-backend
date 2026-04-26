namespace nutrition_app_backend.Extensions;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    // Từ khóa 'this' giúp method này "gắn" thẳng vào đối tượng ClaimsPrincipal (User)
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc thiếu định danh người dùng.");
        }

        return userId;
    }
}