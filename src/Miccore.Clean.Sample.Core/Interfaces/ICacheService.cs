namespace Miccore.Clean.Sample.Core.Interfaces;

/// <summary>
/// Abstraction for caching service operations.
/// Implementations can use in-memory cache, Redis, or other caching providers.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value</typeparam>
    /// <param name="key">The cache key</param>
    /// <returns>The cached value or null if not found</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Sets a value in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiration">Optional expiration time (defaults to implementation default)</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    /// <param name="key">The cache key</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes all cache entries matching a pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match (e.g., "user_*")</param>
    Task RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Clears all cache entries.
    /// </summary>
    Task ClearAllAsync();

    /// <summary>
    /// Gets the total number of items in the cache.
    /// </summary>
    int GetKeyCount();
}
