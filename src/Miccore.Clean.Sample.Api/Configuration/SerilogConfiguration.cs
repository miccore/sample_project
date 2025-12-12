using Serilog;

namespace Miccore.Clean.Sample.Api.Configuration;

/// <summary>
/// Extension methods for configuring Serilog logging.
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Configures Serilog with settings from appsettings.json.
    /// </summary>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));
    }

    /// <summary>
    /// Configures Serilog request logging middleware.
    /// </summary>
    public static IApplicationBuilder UseSerilogRequestLoggingWithContext(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

                if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
                {
                    diagnosticContext.Set("CorrelationId", correlationId ?? "unknown");
                }
            };
        });
    }
}
