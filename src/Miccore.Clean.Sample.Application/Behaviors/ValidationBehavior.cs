using FluentValidation;
using MediatR;
using Miccore.Clean.Sample.Core.Exceptions;

namespace Miccore.Clean.Sample.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that automatically validates requests using FluentValidation.
/// If validation fails, throws a ValidatorException with all validation errors.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Run all validators in parallel for better performance
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            var errorMessages = failures
                .Select(f => $"{f.PropertyName}: {f.ErrorMessage}")
                .ToList();

            throw new ValidatorException(string.Join("\n", errorMessages));
        }

        return await next();
    }
}
