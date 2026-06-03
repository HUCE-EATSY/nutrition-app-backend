using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace nutrition_app_backend.Controllers.Admin;

public class AdminLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AdminAuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AdminLoginRequest request)
    {
        // For development/testing purposes, accept 'admin123' or specific emails
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { success = false, message = "Email and password are required." });
        }

        if (request.Password != "admin123" && request.Password != "123")
        {
            return Unauthorized(new { success = false, message = "Invalid email or password" });
        }

        var key = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(key)) 
        {
            return StatusCode(500, new { success = false, message = "JWT Key is not configured." });
        }

        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()), // Mock Admin ID
            new Claim(JwtRegisteredClaimNames.Email, request.Email),
            new Claim(System.Security.Claims.ClaimTypes.Role, "Admin"), // Use ClaimTypes.Role for [Authorize(Roles)]
            new Claim("role", "admin"), // Also include lowercase for compatibility
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expirationMinutes = int.TryParse(_config["Jwt:ExpirationMinutes"], out int exp) ? exp : 60;

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { success = true, token = tokenString });
    }
}
