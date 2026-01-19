namespace Miccore.Clean.Sample.Api.Services;

/// <summary>
/// Interface for endpoint mapping service.
/// Provides mapping between API models and application DTOs.
/// </summary>
public interface IEndpointMapper
{
    /// <summary>
    /// Maps source object to destination type.
    /// </summary>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps source object to destination type with explicit source type.
    /// </summary>
    TDestination Map<TSource, TDestination>(TSource source);
}
