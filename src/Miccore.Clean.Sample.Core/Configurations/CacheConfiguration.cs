namespace Miccore.Clean.Sample.Core.Configurations;

/// <summary>
/// Configuration settings for caching behavior.
/// Maps to the "Cache" section in appsettings.json.
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Cache";

    /// <summary>
    /// Default cache expiration time in minutes.
    /// </summary>
    public int DefaultExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Repository cache expiration time in minutes.
    /// </summary>
    public int RepositoryExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Sliding expiration time in minutes for cache entries.
    /// </summary>
    public int SlidingExpirationMinutes { get; set; } = 5;

    /// <summary>
    /// Whether caching is enabled globally.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets the default expiration as TimeSpan.
    /// </summary>
    public TimeSpan DefaultExpiration => TimeSpan.FromMinutes(DefaultExpirationMinutes);

    /// <summary>
    /// Gets the repository expiration as TimeSpan.
    /// </summary>
    public TimeSpan RepositoryExpiration => TimeSpan.FromMinutes(RepositoryExpirationMinutes);

    /// <summary>
    /// Gets the sliding expiration as TimeSpan.
    /// </summary>
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(SlidingExpirationMinutes);
}
