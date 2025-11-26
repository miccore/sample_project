using Miccore.Clean.Sample.Api.Middleware;

namespace Miccore.Clean.Sample.Api.Configuration;

/// <summary>
/// Extension methods for configuring custom middleware pipeline.
/// </summary>
public static class MiddlewareConfiguration
{
    /// <summary>
    /// Configures the custom middleware pipeline in the correct order.
    /// </summary>
    /// <remarks>
    /// Middleware order:
    /// 1. CorrelationId - Adds/propagates correlation ID for distributed tracing
    /// 2. ExceptionHandling - Catches and formats all unhandled exceptions
    /// </remarks>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        // Order matters!
        app.UseCorrelationId();      // 1. Add correlation ID first
        app.UseExceptionHandling();  // 2. Then exception handling

        return app;
    }
}
