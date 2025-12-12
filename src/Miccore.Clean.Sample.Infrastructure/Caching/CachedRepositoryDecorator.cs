using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Repositories;
using Miccore.Pagination;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Miccore.Clean.Sample.Infrastructure.Caching;

/// <summary>
/// Decorator that adds caching capabilities to any repository implementing IBaseRepository.
/// Uses cache-aside pattern: read from cache first, write-through on mutations.
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public class CachedRepositoryDecorator<T> : IBaseRepository<T> where T : BaseEntity
{
    private readonly IBaseRepository<T> _innerRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedRepositoryDecorator<T>> _logger;
    private readonly CacheConfiguration _cacheConfig;
    private readonly string _entityName;

    public CachedRepositoryDecorator(
        IBaseRepository<T> innerRepository,
        ICacheService cacheService,
        ILogger<CachedRepositoryDecorator<T>> logger,
        IOptions<CacheConfiguration> cacheConfig)
    {
        _innerRepository = innerRepository ?? throw new ArgumentNullException(nameof(innerRepository));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheConfig = cacheConfig?.Value ?? new CacheConfiguration();
        _entityName = typeof(T).Name;
    }

    private string GetCacheKey(Guid id) => $"repo_{_entityName}_{id}";
    private string GetCacheKeyPattern() => $"repo_{_entityName}_*";

    /// <summary>
    /// Gets entity by ID with caching.
    /// </summary>
    public async Task<T> GetByIdAsync(Guid id, params string[] includes)
    {
        var cacheKey = GetCacheKey(id);

        // Try to get from cache first
        var cached = await _cacheService.GetAsync<T>(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Cache hit for {EntityName} with ID: {Id}", _entityName, id);
            return cached;
        }

        // Cache miss - get from database
        _logger.LogDebug("Cache miss for {EntityName} with ID: {Id}", _entityName, id);
        var entity = await _innerRepository.GetByIdAsync(id, includes);

        // Cache the result
        await _cacheService.SetAsync(cacheKey, entity, _cacheConfig.RepositoryExpiration);

        return entity;
    }

    /// <summary>
    /// Gets entity by parameters (not cached - complex queries).
    /// </summary>
    public async Task<T> GetByParametersAsync(Expression<Func<T, bool>> expression, params string[] includes)
    {
        // Complex queries are not cached
        return await _innerRepository.GetByParametersAsync(expression, includes);
    }

    /// <summary>
    /// Gets all entities with pagination (not cached - list queries).
    /// </summary>
    public async Task<PaginationModel<T>> GetAllAsync(PaginationQuery paginationQuery, params string[] includes)
    {
        // Paginated lists are not cached
        return await _innerRepository.GetAllAsync(paginationQuery, includes);
    }

    /// <summary>
    /// Gets filtered entities with pagination (not cached - complex queries).
    /// </summary>
    public async Task<PaginationModel<T>> GetAllByParametersPaginatedAsync(
        PaginationQuery paginationQuery,
        Expression<Func<T, bool>> expression,
        params string[] includes)
    {
        return await _innerRepository.GetAllByParametersPaginatedAsync(paginationQuery, expression, includes);
    }

    /// <summary>
    /// Gets filtered entities (not cached - list queries).
    /// </summary>
    public async Task<List<T>> GetAllByParametersAsync(
        Expression<Func<T, bool>> expression,
        params string[] includes)
    {
        return await _innerRepository.GetAllByParametersAsync(expression, includes);
    }

    /// <summary>
    /// Adds entity and invalidates related caches.
    /// </summary>
    public async Task<T> AddAsync(T entity)
    {
        var result = await _innerRepository.AddAsync(entity);

        // Cache the new entity
        await _cacheService.SetAsync(GetCacheKey(result.Id), result, _cacheConfig.RepositoryExpiration);

        _logger.LogDebug("Added {EntityName} with ID: {Id} and cached", _entityName, result.Id);

        return result;
    }

    /// <summary>
    /// Updates entity and invalidates cache.
    /// </summary>
    public async Task<T> UpdateAsync(T entity)
    {
        var result = await _innerRepository.UpdateAsync(entity);

        // Update cache with new value
        await _cacheService.SetAsync(GetCacheKey(result.Id), result, _cacheConfig.RepositoryExpiration);

        _logger.LogDebug("Updated {EntityName} with ID: {Id} and refreshed cache", _entityName, result.Id);

        return result;
    }

    /// <summary>
    /// Soft deletes entity and invalidates cache.
    /// </summary>
    public async Task<T> DeleteAsync(Guid id)
    {
        var result = await _innerRepository.DeleteAsync(id);

        // Remove from cache
        await _cacheService.RemoveAsync(GetCacheKey(id));

        _logger.LogDebug("Soft deleted {EntityName} with ID: {Id} and invalidated cache", _entityName, id);

        return result;
    }

    /// <summary>
    /// Hard deletes entities matching expression and invalidates all caches for this entity type.
    /// </summary>
    public async Task DeleteHardAsync(Expression<Func<T, bool>> expression)
    {
        await _innerRepository.DeleteHardAsync(expression);

        // Invalidate all caches for this entity type
        await _cacheService.RemoveByPatternAsync(GetCacheKeyPattern());

        _logger.LogDebug("Hard deleted {EntityName} entities matching expression and invalidated all related caches", _entityName);
    }
}
