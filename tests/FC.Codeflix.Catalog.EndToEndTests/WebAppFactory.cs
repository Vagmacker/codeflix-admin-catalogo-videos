using Testcontainers.MySql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests;

public class WebAppFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _dbContainer = new MySqlBuilder()
        .WithImage("mysql:latest")
        .WithUsername("root")
        .WithPassword("123456")
        .WithDatabase("codeflix")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(service =>
                service.ServiceType == typeof(DbContextOptions<CodeflixCatalogDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            var mysqlConnectionString = _dbContainer.GetConnectionString();
            services.AddDbContext<CodeflixCatalogDbContext>(options =>
            {
                options
                    .UseMySql(mysqlConnectionString, ServerVersion.AutoDetect(mysqlConnectionString));
            });
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}