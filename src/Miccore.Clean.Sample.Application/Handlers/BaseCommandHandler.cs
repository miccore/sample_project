namespace Miccore.Clean.Sample.Application.Handlers;

/// <summary>
/// Base handler for commands that provides standardized request validation.
/// Logging is handled by the LoggingBehavior pipeline.
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class BaseCommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        ArgumentNullException.ThrowIfNull(request);

        // Execute the command
        return await HandleCommand(request, cancellationToken);
    }

    /// <summary>
    /// Override this method to implement the command logic.
    /// </summary>
    protected abstract Task<TResponse> HandleCommand(TCommand request, CancellationToken cancellationToken);
}
