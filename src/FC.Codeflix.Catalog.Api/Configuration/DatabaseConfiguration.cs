using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConnection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("AdminVideo");

        services.AddDbContext<CodeflixCatalogDbContext>(
            options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == "EndToEndTest")
            return app;

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CodeflixCatalogDbContext>();

        dbContext.Database.Migrate();

        return app;
    }
}