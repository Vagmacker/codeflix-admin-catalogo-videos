using Microsoft.OpenApi.Models;
using FC.Codeflix.Catalog.Api.Configuration.Json;
using FC.Codeflix.Catalog.Api.Controllers.Filters;

namespace FC.Codeflix.Catalog.Api.Configuration;

public static class ControllerConfiguration
{
    public static IServiceCollection AddAndConfigureControllers(
        this IServiceCollection services
    )
    {
        services
            .AddControllers(options => options.Filters.Add(typeof(GlobalExceptionFilter)))
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy());

        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddDocumentation();

        return services;
    }

    private static IServiceCollection AddDocumentation(
        this IServiceCollection services
    )
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.EnableAnnotations();
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "FC3 Codeflix Catalog", Version = "V1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static WebApplication UseDocumentation(
        this WebApplication app
    )
    {
        if (!app.Environment.IsDevelopment()) return app;
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}