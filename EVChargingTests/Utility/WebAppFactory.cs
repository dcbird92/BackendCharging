using EVCharging.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EVChargingTests.Utility;

/// <summary>
/// Custom WebApplicationFactory used for integration testing.
/// Replaces the real database with an InMemory database and boots the full API pipeline,
/// allowing real HTTP requests to be executed against the application.
/// </summary>
public class WebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<EvChargingDbContext>));

            services.Remove(descriptor);

            // Add InMemory DB for controller tests
            services.AddDbContext<EvChargingDbContext>(options =>
            {
                options.UseInMemoryDatabase("EVChargingDb");
            });
        });
    }
}
