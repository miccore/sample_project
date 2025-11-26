using System.Net;
using System.Text.Json;
using Miccore.Clean.Sample.Core.ApiModels;
using Miccore.Clean.Sample.Core.Exceptions;

namespace Miccore.Clean.Sample.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and returns a standardized ApiResponse with appropriate HTTP status codes.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";
        
        var (statusCode, message) = exception switch
        {
            NotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message),
            ValidatorException validatorEx => (HttpStatusCode.BadRequest, validatorEx.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
            ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
            OperationCanceledException => (HttpStatusCode.BadRequest, "Operation was cancelled"),
            _ => (HttpStatusCode.InternalServerError, GetErrorMessage(exception))
        };

        // Log with appropriate level based on exception type
        LogException(exception, correlationId, statusCode);

        // Build the response
        var response = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = GetTitle(statusCode),
            Detail = message,
            Instance = context.Request.Path,
            Extensions = 
            {
                ["correlationId"] = correlationId,
                ["traceId"] = context.TraceIdentifier
            }
        };

        // Add stack trace in development
        if (_environment.IsDevelopment() && exception is not (NotFoundException or ValidatorException))
        {
            response.Extensions["stackTrace"] = exception.StackTrace;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }

    private string GetErrorMessage(Exception exception)
    {
        // In production, don't expose internal error details
        if (_environment.IsProduction())
        {
            return "An unexpected error occurred. Please try again later.";
        }
        
        return exception.Message;
    }

    private static string GetTitle(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.NotFound => "Resource Not Found",
        HttpStatusCode.BadRequest => "Bad Request",
        HttpStatusCode.Unauthorized => "Unauthorized",
        HttpStatusCode.Forbidden => "Forbidden",
        HttpStatusCode.InternalServerError => "Internal Server Error",
        _ => "Error"
    };

    private void LogException(Exception exception, string correlationId, HttpStatusCode statusCode)
    {
        var logLevel = statusCode switch
        {
            HttpStatusCode.InternalServerError => LogLevel.Error,
            HttpStatusCode.BadRequest => LogLevel.Warning,
            HttpStatusCode.NotFound => LogLevel.Information,
            _ => LogLevel.Warning
        };

        _logger.Log(
            logLevel,
            exception,
            "Exception occurred. CorrelationId: {CorrelationId}, StatusCode: {StatusCode}, Message: {Message}",
            correlationId,
            (int)statusCode,
            exception.Message);
    }
}

/// <summary>
/// Problem details response following RFC 7807 standard.
/// </summary>
public class ProblemDetails
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public Dictionary<string, object?> Extensions { get; set; } = new();
}

/// <summary>
/// Extension method to register the ExceptionHandlingMiddleware.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
