using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace nutrition_app_backend.Data;

/// <summary>
/// Factory này chỉ dùng khi chạy lệnh EF CLI (migrations add/update).
/// Dùng localhost thay vì server thật để không cần VPN/kết nối server khi tạo migration.
/// </summary>
public class WaoDbContextFactory : IDesignTimeDbContextFactory<WaoDbContext>
{
    public WaoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WaoDbContext>();

        // Connection string tạm cho design-time (chỉ cần cú pháp đúng, không cần kết nối thật)
        var connectionString = "Server=localhost;Port=3306;Database=wao_health_app;User=root;Password=placeholder;";

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.Parse("8.0.0-mysql")
        );

        return new WaoDbContext(optionsBuilder.Options);
    }
}
