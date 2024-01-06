using System.Reflection;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Application.Category.Create;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Domain.Category;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.Api.Configuration;

public static class UseCaseConfiguration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateCategoryUseCase).Assembly)
        );
        services.AddRepositories();
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();

        return services;
    }
}