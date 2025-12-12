using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Miccore.Clean.Sample.Core.ApiModels;
using Miccore.Clean.Sample.Core.Exceptions;

namespace Miccore.Clean.Sample.Api.Endpoints;

/// <summary>
/// Base endpoint providing common functionality for all API endpoints.
/// Handles validation failures, exception handling (via middleware), and standardized response formatting.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, ApiResponse<TResponse>>
    where TRequest : notnull
    where TResponse : class, new()
{
    /// <summary>
    /// Gets the API version for this endpoint.
    /// Override in derived classes for different versions.
    /// </summary>
    protected virtual string ApiVersion => "v1";

    /// <summary>
    /// Gets the base route prefix for versioned API endpoints.
    /// </summary>
    protected string VersionedRoutePrefix => $"api/{ApiVersion}";

    /// <summary>
    /// Builds the full versioned route.
    /// </summary>
    /// <param name="route">The relative route (e.g., "samples" or "samples/{id}").</param>
    /// <returns>The full versioned route (e.g., "api/v1/samples").</returns>
    protected string BuildRoute(string route) => $"{VersionedRoutePrefix}/{route.TrimStart('/')}";

    /// <summary>
    /// Handles the request by checking validation and delegating to the derived implementation.
    /// Note: Exception handling is now managed by ExceptionHandlingMiddleware.
    /// </summary>
    public override async Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        // Check for validation failures from FastEndpoints validators
        if (ValidationFailed)
        {
            var failures = ValidationFailures
                .Select(x => $"{x.PropertyName}: {x.ErrorMessage}")
                .ToList();

            throw new ValidatorException(string.Join("\n", failures));
        }

        // Delegate to derived class implementation
        // Exceptions are caught by ExceptionHandlingMiddleware
        await ExecuteAsync(request, cancellationToken);
    }

    /// <summary>
    /// Override this method to implement the endpoint logic.
    /// No need for try-catch as exceptions are handled by middleware.
    /// </summary>
    /// <param name="request">The validated request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected new abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a successful response with HTTP 200 OK.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task SendSuccessAsync(TResponse data, CancellationToken cancellationToken = default)
    {
        var response = ApiResponse<TResponse>.Success(data);
        Response = response;
        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    }

    /// <summary>
    /// Sends a successful response with HTTP 201 Created.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task SendCreatedAsync(TResponse data, CancellationToken cancellationToken = default)
    {
        var response = ApiResponse<TResponse>.Success(data);
        Response = response;
        HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
        await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    }

    /// <summary>
    /// Sends a successful response with HTTP 204 No Content.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected Task SendNoContentAsync(CancellationToken cancellationToken = default)
    {
        HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sends a paginated response with HTTP 200 OK.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected async Task SendPaginatedAsync(TResponse data, CancellationToken cancellationToken = default)
    {
        var response = ApiResponse<TResponse>.Success(data);
        Response = response;
        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    }
}

/// <summary>
/// Base endpoint for requests that don't return data (commands without response).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public abstract class BaseEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : notnull
{
    /// <summary>
    /// Gets the API version for this endpoint.
    /// </summary>
    protected virtual string ApiVersion => "v1";

    /// <summary>
    /// Gets the base route prefix for versioned API endpoints.
    /// </summary>
    protected string VersionedRoutePrefix => $"api/{ApiVersion}";

    /// <summary>
    /// Builds the full versioned route.
    /// </summary>
    protected string BuildRoute(string route) => $"{VersionedRoutePrefix}/{route.TrimStart('/')}";

    /// <summary>
    /// Handles the request by checking validation and delegating to the derived implementation.
    /// </summary>
    public override async Task HandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (ValidationFailed)
        {
            var failures = ValidationFailures
                .Select(x => $"{x.PropertyName}: {x.ErrorMessage}")
                .ToList();

            throw new ValidatorException(string.Join("\n", failures));
        }

        await ExecuteAsync(request, cancellationToken);
    }

    /// <summary>
    /// Override this method to implement the endpoint logic.
    /// </summary>
    protected new abstract Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a successful response with HTTP 204 No Content.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected Task SendNoContentAsync(CancellationToken cancellationToken = default)
    {
        HttpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
        return Task.CompletedTask;
    }
}

/// <summary>
/// Base endpoint for endpoints without request body (GET by ID, DELETE).
/// </summary>
public abstract class BaseEndpointWithoutRequest<TResponse> : EndpointWithoutRequest<ApiResponse<TResponse>>
    where TResponse : class, new()
{
    /// <summary>
    /// Gets the API version for this endpoint.
    /// </summary>
    protected virtual string ApiVersion => "v1";

    /// <summary>
    /// Gets the base route prefix for versioned API endpoints.
    /// </summary>
    protected string VersionedRoutePrefix => $"api/{ApiVersion}";

    /// <summary>
    /// Builds the full versioned route.
    /// </summary>
    protected string BuildRoute(string route) => $"{VersionedRoutePrefix}/{route.TrimStart('/')}";

    /// <summary>
    /// Sends a successful response with HTTP 200 OK.
    /// </summary>
    protected async Task SendSuccessAsync(TResponse data, CancellationToken cancellationToken = default)
    {
        var response = ApiResponse<TResponse>.Success(data);
        Response = response;
        HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
        await HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    }
}
