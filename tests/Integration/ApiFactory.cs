using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerApi.Data;

namespace TaskManagerApi.Tests.Integration;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"test_{Guid.NewGuid()}.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            var dbName = _dbName;
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbName}"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (File.Exists(_dbName))
            File.Delete(_dbName);
    }
}
