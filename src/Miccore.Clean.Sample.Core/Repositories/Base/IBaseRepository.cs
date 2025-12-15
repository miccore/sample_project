namespace Miccore.Clean.Sample.Core.Repositories.Base;

/// <summary>
/// Base repository interface for write operations.
/// Inherits read operations from IReadOnlyRepository.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IBaseRepository<T> : IReadOnlyRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Adds a new entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted entity.</returns>
    Task<T> DeleteAsync(Guid id);

    /// <summary>
    /// Deletes entities by parameters asynchronously.
    /// </summary>
    /// <param name="whereExpression">The expression to filter the entities to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteHardAsync(Expression<Func<T, bool>> whereExpression);
}
