using Miccore.Clean.Sample.Api.Configuration;
using Miccore.Clean.Sample.Application;
using Miccore.Clean.Sample.Infrastructure;
using Serilog;

// Configure Serilog early for bootstrap logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting application...");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.ConfigureSerilog();

    // configure serviceDefaults
    builder.AddServiceDefaults();

    // Add services
    builder.Services
        .AddEndpointServices()
        .AddApplication(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddHealthCheckServices();

    var app = builder.Build();

    // Configure middleware pipeline
    app.UseSerilogRequestLoggingWithContext();
    app.UseCustomMiddleware();
    app.UseEndpointConfiguration();

    // Map endpoints
    app.MapHealthCheckEndpoints();
    app.MapApiDocumentation(app.Environment);
    app.MapOpenApi();

    app.UseHttpsRedirection();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
