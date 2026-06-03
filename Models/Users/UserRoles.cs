namespace nutrition_app_backend.Models.Users;

public static class UserRoles
{
    // Role byte values (database)
    public const byte UserValue = 1;
    public const byte AdminValue = 2;
    public const byte PremiumValue = 3;
    
    // Role string names (JWT claims and authorization)
    public const string User = "User";
    public const string Admin = "Admin";
    public const string Premium = "Premium";
    
    // Extension method for User.Role conversion
    public static string ToRoleString(this byte roleValue)
    {
        return roleValue switch
        {
            UserValue => User,
            AdminValue => Admin,
            PremiumValue => Premium,
            _ => User // Default to User for unknown values
        };
    }
}
