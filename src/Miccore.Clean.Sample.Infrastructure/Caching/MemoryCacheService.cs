using System.Collections.Concurrent;

namespace Miccore.Clean.Sample.Infrastructure.Caching;

/// <summary>
/// In-memory cache service implementation using IMemoryCache.
/// Suitable for single-instance deployments. For distributed systems, use Redis alternative.
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly CacheConfiguration _cacheConfig;

    /// <summary>
    /// Thread-safe collection to track cache keys for pattern-based removal.
    /// Uses ConcurrentDictionary for lock-free operations (value is unused, using byte for minimal memory).
    /// </summary>
    private static readonly ConcurrentDictionary<string, byte> CacheKeys = new();

    public MemoryCacheService(
        IMemoryCache cache,
        ILogger<MemoryCacheService> logger,
        IOptions<CacheConfiguration> cacheConfig)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheConfig = cacheConfig?.Value ?? new CacheConfiguration();
    }

    /// <summary>
    /// Retrieves a value from the cache by key.
    /// </summary>
    /// <typeparam name="T">The type of value to retrieve</typeparam>
    /// <param name="key">The cache key</param>
    /// <returns>The cached value or null if not found or expired</returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            var found = _cache.TryGetValue(key, out T? value);

            if (found && value != null)
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", MaskSensitiveData(key));
                return await Task.FromResult(value);
            }
            else
            {
                _logger.LogDebug("Cache miss for key: {CacheKey}", MaskSensitiveData(key));
                return await Task.FromResult<T?>(default);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from cache for key: {CacheKey}", MaskSensitiveData(key));
            return await Task.FromResult<T?>(default);
        }
    }

    /// <summary>
    /// Sets a value in the cache with optional expiration.
    /// </summary>
    /// <typeparam name="T">The type of value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiration">Optional expiration time (uses configured default if not provided)</param>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        try
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _cacheConfig.DefaultExpiration,
                SlidingExpiration = _cacheConfig.SlidingExpiration
            };

            _cache.Set(key, value, cacheOptions);
            CacheKeys.TryAdd(key, 0);

            _logger.LogDebug("Cache set for key: {CacheKey} with expiration: {Expiration}",
                MaskSensitiveData(key), expiration ?? _cacheConfig.DefaultExpiration);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {CacheKey}", MaskSensitiveData(key));
        }
    }

    /// <summary>
    /// Removes a specific key from the cache.
    /// </summary>
    /// <param name="key">The cache key to remove</param>
    public async Task RemoveAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            _cache.Remove(key);
            CacheKeys.TryRemove(key, out _);

            _logger.LogDebug("Cache removed for key: {CacheKey}", MaskSensitiveData(key));
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from cache for key: {CacheKey}", MaskSensitiveData(key));
        }
    }

    /// <summary>
    /// Removes all cache keys matching a pattern (e.g., "repo_sample_*").
    /// Pattern matching uses simple wildcard support (* for any characters).
    /// </summary>
    /// <param name="pattern">The pattern to match (e.g., "prefix_*")</param>
    public async Task RemoveByPatternAsync(string pattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        try
        {
            var keysToRemove = CacheKeys.Keys
                .Where(k => MatchesPattern(k, pattern))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                CacheKeys.TryRemove(key, out _);
            }

            _logger.LogDebug("Pattern-based cache removal completed for pattern: {Pattern} | Removed {Count} keys",
                pattern, keysToRemove.Count);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
        }
    }

    /// <summary>
    /// Clears all cache entries. Use with caution in production.
    /// </summary>
    public async Task ClearAllAsync()
    {
        try
        {
            foreach (var key in CacheKeys.Keys.ToList())
            {
                _cache.Remove(key);
                CacheKeys.TryRemove(key, out _);
            }

            _logger.LogWarning("All cache entries cleared");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all cache");
        }
    }

    /// <summary>
    /// Gets the total number of keys in the cache.
    /// </summary>
    public int GetKeyCount() => CacheKeys.Count;

    private static bool MatchesPattern(string key, string pattern)
    {
        if (pattern == "*")
            return true;

        // Simple wildcard pattern matching
        var patternParts = pattern.Split('*');
        var currentIndex = 0;

        foreach (var part in patternParts)
        {
            if (string.IsNullOrEmpty(part))
                continue;

            var index = key.IndexOf(part, currentIndex, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return false;

            currentIndex = index + part.Length;
        }

        return true;
    }

    private static string MaskSensitiveData(string key)
    {
        // Simple masking to avoid logging sensitive key details
        return key.Length > 20 ? $"{key[..10]}...{key[^10..]}" : key;
    }
}
