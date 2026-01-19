namespace Miccore.Clean.Sample.Api.Services;

/// <summary>
/// Interface for formatting HTTP responses.
/// Handles the responsibility of creating and sending standardized API responses.
/// </summary>
public interface IResponseFormatter
{
    /// <summary>
    /// Sends a successful response with HTTP 200 OK.
    /// </summary>
    Task SendSuccessAsync<TResponse>(HttpContext context, TResponse data, CancellationToken cancellationToken = default)
        where TResponse : class, new();

    /// <summary>
    /// Sends a successful response with HTTP 201 Created.
    /// </summary>
    Task SendCreatedAsync<TResponse>(HttpContext context, TResponse data, CancellationToken cancellationToken = default)
        where TResponse : class, new();

    /// <summary>
    /// Sends a successful response with HTTP 204 No Content.
    /// </summary>
    Task SendNoContentAsync(HttpContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a paginated response with HTTP 200 OK.
    /// </summary>
    Task SendPaginatedAsync<TResponse>(HttpContext context, TResponse data, CancellationToken cancellationToken = default)
        where TResponse : class, new();
}
