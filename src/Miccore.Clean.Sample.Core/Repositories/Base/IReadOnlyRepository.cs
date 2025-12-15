namespace Miccore.Clean.Sample.Core.Repositories.Base;

/// <summary>
/// Read-only repository interface for query operations.
/// Follows Interface Segregation Principle (ISP) by separating read operations from write operations.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IReadOnlyRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets all entities asynchronously with pagination and optional includes.
    /// </summary>
    /// <param name="query">The pagination query.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list of entities.</returns>
    Task<PaginationModel<T>> GetAllAsync(PaginationQuery query, params string[] includes);

    /// <summary>
    /// Gets an entity by its identifier asynchronously with optional includes.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
    Task<T> GetByIdAsync(Guid id, params string[] includes);

    /// <summary>
    /// Gets all entities by parameters with pagination asynchronously with optional includes.
    /// </summary>
    /// <param name="query">The pagination query.</param>
    /// <param name="whereExpression">The expression to filter the entities.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list of entities.</returns>
    Task<PaginationModel<T>> GetAllByParametersPaginatedAsync(PaginationQuery query, Expression<Func<T, bool>> whereExpression, params string[] includes);

    /// <summary>
    /// Gets all entities by parameters asynchronously with optional includes.
    /// </summary>
    /// <param name="whereExpression">The expression to filter the entities.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of entities.</returns>
    Task<List<T>> GetAllByParametersAsync(Expression<Func<T, bool>> whereExpression, params string[] includes);

    /// <summary>
    /// Gets an entity by parameters asynchronously with optional includes.
    /// </summary>
    /// <param name="whereExpression">The expression to filter the entity.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity.</returns>
    Task<T> GetByParametersAsync(Expression<Func<T, bool>> whereExpression, params string[] includes);
}
