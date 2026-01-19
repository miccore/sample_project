using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace Miccore.Clean.Sample.Api.Configuration;

/// <summary>
/// Extension methods for configuring FastEndpoints and API documentation.
/// </summary>
public static class EndpointConfiguration
{
    /// <summary>
    /// Adds FastEndpoints and Swagger documentation services.
    /// </summary>
    public static IServiceCollection AddEndpointServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure API options from configuration
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));

        var apiOptions = configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>() ?? new ApiOptions();

        services.AddOpenApi();
        services.AddFastEndpoints().SwaggerDocument(o =>
        {
            o.DocumentSettings = s =>
            {
                s.DocumentName = apiOptions.DefaultVersion;
                s.Title = apiOptions.SwaggerTitle;
                s.Version = apiOptions.DefaultVersion;
                s.Description = apiOptions.SwaggerDescription;
            };
            o.EnableJWTBearerAuth = true;
            o.ShortSchemaNames = true;
            o.ExcludeNonFastEndpoints = true;
        });

        return services;
    }

    /// <summary>
    /// Configures FastEndpoints routing and Swagger middleware.
    /// </summary>
    public static IApplicationBuilder UseEndpointConfiguration(this IApplicationBuilder app)
    {
        var apiOptions = app.ApplicationServices.GetService<IOptions<ApiOptions>>()?.Value ?? new ApiOptions();

        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = apiOptions.RoutePrefix;
        }).UseSwaggerGen();

        return app;
    }

    /// <summary>
    /// Maps API documentation endpoints for development environment.
    /// </summary>
    public static IEndpointRouteBuilder MapApiDocumentation(this IEndpointRouteBuilder app, IWebHostEnvironment environment)
    {
        var apiOptions = app.ServiceProvider.GetService<IOptions<ApiOptions>>()?.Value ?? new ApiOptions();

        if (environment.IsDevelopment() && apiOptions.EnableSwagger)
        {
            app.MapScalarApiReference();
        }

        return app;
    }
}
