using System.Net;
using Microsoft.AspNetCore.Http;

namespace Miccore.Clean.Sample.Api.Services;

/// <summary>
/// Default implementation of IResponseFormatter.
/// Formats responses using the ApiResponse wrapper.
/// </summary>
public class ApiResponseFormatter : IResponseFormatter
{
    public async Task SendSuccessAsync<TResponse>(HttpContext context, TResponse data, CancellationToken cancellationToken = default)
        where TResponse : class, new()
    {
        var response = ApiResponse<TResponse>.Success(data);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await context.Response.WriteAsJsonAsync(response, cancellationToken);
    }

    public async Task SendCreatedAsync<TResponse>(HttpContext context, TResponse data, CancellationToken cancellationToken = default)
        where TResponse : class, new()
    {
        var response = ApiResponse<TResponse>.Success(data);
        context.Response.StatusCode = (int)HttpStatusCode.Created;
        await context.Response.WriteAsJsonAsync(response, cancellationToken);
    }

    public Task SendNoContentAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        return Task.CompletedTask;
    }

    public async Task SendPaginatedAsync<TResponse>(HttpContext context, TResponse data, CancellationToken cancellationToken = default)
        where TResponse : class, new()
    {
        var response = ApiResponse<TResponse>.Success(data);
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await context.Response.WriteAsJsonAsync(response, cancellationToken);
    }
}
