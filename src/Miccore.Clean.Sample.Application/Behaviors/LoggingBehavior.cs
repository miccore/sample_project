using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Miccore.Clean.Sample.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs request/response with timing information.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8];

        _logger.LogInformation(
            "[{RequestId}] Handling {RequestName}",
            requestId,
            requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "[{RequestId}] Handled {RequestName} in {ElapsedMilliseconds}ms",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds);

            // Log warning for slow requests (> 500ms)
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "[{RequestId}] Long running request {RequestName} took {ElapsedMilliseconds}ms",
                    requestId,
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "[{RequestId}] Request {RequestName} failed after {ElapsedMilliseconds}ms",
                requestId,
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
