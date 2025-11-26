namespace Miccore.Clean.Sample.Infrastructure.Repositories.Base
{
    /// <summary>
    /// Implementation of the core repository interface.
    /// Soft delete filtering is handled automatically by the global query filter in DbContext.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class BaseRepository<T>(SampleApplicationDbContext context) : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly SampleApplicationDbContext _context = context;

        /// <summary>
        /// Adds an entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Soft deletes an entity by setting its DeletedAt property.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>The soft deleted entity.</returns>
        public async Task<T> DeleteAsync(Guid id)
        {
            var entity = await _context.Set<T>()
                                        .FirstOrDefaultAsync(x => x.Id == id) 
                            ?? throw new NotFoundException(ExceptionEnum.NotFound.GetEnumDescription());
        
            entity.DeletedAt = DateHelper.GetCurrentTimestamp();
        
            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Hard deletes entities that match the given expression.
        /// Uses IgnoreQueryFilters to include soft-deleted entities.
        /// </summary>
        /// <param name="WhereExpression">The expression to filter entities.</param>
        public async Task DeleteHardAsync(Expression<Func<T, bool>> WhereExpression)
        {
            var entity = await _context.Set<T>()
                                    .AsNoTracking()
                                    .IgnoreQueryFilters()
                                    .Where(WhereExpression)
                                    .ToListAsync();

            _context.RemoveRange(entity);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all entities with pagination.
        /// </summary>
        /// <param name="query">The pagination query parameters.</param>
        /// <param name="includes">Related entities to include.</param>
        /// <returns>A paginated list of entities.</returns>
        public async Task<PaginationModel<T>> GetAllAsync(PaginationQuery query, params string[] includes)
        {
            var entities = await _context.Set<T>()
                                        .AsNoTracking()
                                        .ApplyIncludes(includes)
                                        .PaginateAsync(query);
    
            return entities;
        }

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <param name="includes">Related entities to include.</param>
        /// <returns>The entity with the given ID.</returns>
        public async Task<T> GetByIdAsync(Guid id, params string[] includes)
        {
            var entity = await _context.Set<T>()
                                        .ApplyIncludes(includes)
                                        .FirstOrDefaultAsync(x => x.Id == id) 
                            ?? throw new NotFoundException(ExceptionEnum.NotFound.GetEnumDescription());
            return entity;
        }

        /// <summary>
        /// Retrieves entities that match the given expression with pagination.
        /// </summary>
        /// <param name="query">The pagination query parameters.</param>
        /// <param name="WhereExpression">The expression to filter entities.</param>
        /// <param name="includes">Related entities to include.</param>
        /// <returns>A paginated list of entities.</returns>
        public async Task<PaginationModel<T>> GetAllByParametersPaginatedAsync(PaginationQuery query, Expression<Func<T, bool>> WhereExpression, params string[] includes)
        {
            var entities = await _context.Set<T>()
                                        .AsNoTracking()
                                        .ApplyIncludes(includes)
                                        .Where(WhereExpression)
                                        .PaginateAsync(query);
            return entities;
        }

        /// <summary>
        /// Retrieves entities that match the given expression.
        /// </summary>
        /// <param name="WhereExpression">The expression to filter entities.</param>
        /// <param name="includes">Related entities to include.</param>
        /// <returns>A list of entities.</returns>
        public async Task<List<T>> GetAllByParametersAsync(Expression<Func<T, bool>> WhereExpression, params string[] includes)
        {
            var entities = await _context.Set<T>()
                                        .AsNoTracking()
                                        .ApplyIncludes(includes)
                                        .Where(WhereExpression)
                                        .ToListAsync();
            return entities;
        }

        /// <summary>
        /// Retrieves an entity that matches the given expression.
        /// </summary>
        /// <param name="WhereExpression">The expression to filter entities.</param>
        /// <param name="includes">Related entities to include.</param>
        /// <returns>The entity that matches the expression.</returns>
        public async Task<T> GetByParametersAsync(Expression<Func<T, bool>> WhereExpression, params string[] includes)
        {
            var entity = await _context.Set<T>()
                                        .AsNoTracking()
                                        .ApplyIncludes(includes)
                                        .Where(WhereExpression)
                                        .FirstOrDefaultAsync()
                            ?? throw new NotFoundException(ExceptionEnum.NotFound.GetEnumDescription());
        
            return entity;
        }

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public async Task<T> UpdateAsync(T entity) 
        {
            var existingEntity = await _context.Set<T>().FindAsync(entity.Id) 
                            ?? throw new NotFoundException(ExceptionEnum.NotFound.GetEnumDescription());
            existingEntity.SetUpdatedValues(entity); 
            existingEntity.UpdatedAt = DateHelper.GetCurrentTimestamp();
        
            await _context.SaveChangesAsync();

            return existingEntity;
        }
    }
}