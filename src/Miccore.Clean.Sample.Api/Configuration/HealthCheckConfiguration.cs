using System.Text.Json;
using Miccore.Clean.Sample.Infrastructure.Persistance;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Miccore.Clean.Sample.Api.Configuration;

/// <summary>
/// Extension methods for configuring health checks.
/// </summary>
public static class HealthCheckConfiguration
{
    /// <summary>
    /// Adds health check services including database and self checks.
    /// </summary>
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<SampleApplicationDbContext>(
                name: "database",
                tags: new[] { "db", "sql", "ready" })
            .AddCheck("self", () => HealthCheckResult.Healthy(),
                tags: new[] { "live" });

        return services;
    }

    /// <summary>
    /// Maps health check endpoints for liveness, readiness, and overall health.
    /// </summary>
    public static IEndpointRouteBuilder MapHealthCheckEndpoints(this IEndpointRouteBuilder app)
    {
        // Liveness probe - indicates the app is running
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = WriteHealthCheckResponse
        });

        // Readiness probe - indicates the app is ready to accept traffic
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse
        });

        // Full health check - all checks
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponse
        });

        return app;
    }

    /// <summary>
    /// Custom health check response writer that returns JSON with detailed health check information.
    /// </summary>
    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            entries = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                tags = e.Value.Tags,
                exception = e.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
