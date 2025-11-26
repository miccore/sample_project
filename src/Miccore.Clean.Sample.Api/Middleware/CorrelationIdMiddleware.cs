namespace Miccore.Clean.Sample.Api.Middleware;

/// <summary>
/// Middleware that ensures each request has a correlation ID for distributed tracing.
/// If the request contains an X-Correlation-ID header, it will be used; otherwise, a new one is generated.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation ID from incoming request header
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

        // Generate a new correlation ID if not provided
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        // Store in HttpContext for use throughout the request pipeline
        context.Items["CorrelationId"] = correlationId;

        // Add correlation ID to response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Push correlation ID to Serilog's LogContext for structured logging
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogDebug("Request started with CorrelationId: {CorrelationId}", correlationId);
            await _next(context);
        }
    }
}

/// <summary>
/// Extension method to register the CorrelationIdMiddleware.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}
