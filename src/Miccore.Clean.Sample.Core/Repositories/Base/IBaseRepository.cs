namespace Miccore.Clean.Sample.Core.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
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
}