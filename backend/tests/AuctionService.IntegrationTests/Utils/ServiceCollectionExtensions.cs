using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(TDbContext));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    public static void EnsureDbCreated<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        db.Database.Migrate();
        DbHelper.InitDbForTests(db);
    }
}
