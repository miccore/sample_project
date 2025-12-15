namespace Miccore.Clean.Sample.Application.Handlers;

/// <summary>
/// Base handler for queries that provides standardized request validation.
/// Logging is handled by the LoggingBehavior pipeline.
/// </summary>
/// <typeparam name="TQuery">The query type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class BaseQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await HandleQuery(request, cancellationToken);
    }

    /// <summary>
    /// Override this method to implement the query logic.
    /// </summary>
    protected abstract Task<TResponse> HandleQuery(TQuery request, CancellationToken cancellationToken);
}
