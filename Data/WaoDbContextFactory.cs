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
        var basePath = Directory.GetCurrentDirectory();
        
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<WaoDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );

        return new WaoDbContext(optionsBuilder.Options);
    }
}
