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
    public static IServiceCollection AddEndpointServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddFastEndpoints().SwaggerDocument();
        
        return services;
    }

    /// <summary>
    /// Configures FastEndpoints routing and Swagger middleware.
    /// </summary>
    public static IApplicationBuilder UseEndpointConfiguration(this IApplicationBuilder app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.RoutePrefix = "api";
        }).UseSwaggerGen();

        return app;
    }

    /// <summary>
    /// Maps API documentation endpoints for development environment.
    /// </summary>
    public static IEndpointRouteBuilder MapApiDocumentation(this IEndpointRouteBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.MapScalarApiReference();
        }

        return app;
    }
}
