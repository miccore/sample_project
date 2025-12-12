using System.Linq.Expressions;
using Miccore.Clean.Sample.Core.Entities.Base;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Helpers;
using Miccore.Clean.Sample.Core.Repositories.Base;
using Miccore.Clean.Sample.Infrastructure.Tests.Fixtures;
using Miccore.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Repositories;

/// <summary>
/// Test implementation of IBaseRepository using SQLite for testing.
/// This mirrors the production BaseRepository but uses SqliteTestDbContext.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class SqliteTestRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly SqliteTestDbContext _context;

    public SqliteTestRepository(SqliteTestDbContext context)
    {
        _context = context;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> DeleteAsync(Guid id)
    {
        var entity = await _context.Set<T>()
                                    .FirstOrDefaultAsync(x => x.Id == id && (x.DeletedAt == 0 || x.DeletedAt == null))
                                    ?? throw new NotFoundException("NOT_FOUND");

        entity.DeletedAt = DateHelper.GetCurrentTimestamp();

        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task DeleteHardAsync(Expression<Func<T, bool>> whereExpression)
    {
        var entities = await _context.Set<T>()
                                .Where(whereExpression)
                                .Where(x => x.DeletedAt == 0 || x.DeletedAt == null)
                                .ToListAsync();

        _context.Set<T>().RemoveRange(entities);

        await _context.SaveChangesAsync();
    }

    public async Task<PaginationModel<T>> GetAllAsync(PaginationQuery query, params string[] includes)
    {
        var queryable = _context.Set<T>()
                                .AsNoTracking()
                                .ApplyIncludes(includes)
                                .Where(x => x.DeletedAt == 0 || x.DeletedAt == null);

        return await PaginateAsync(queryable, query);
    }

    public async Task<T> GetByIdAsync(Guid id, params string[] includes)
    {
        var entity = await _context.Set<T>()
                                    .AsNoTracking()
                                    .ApplyIncludes(includes)
                                    .FirstOrDefaultAsync(x => x.Id == id && (x.DeletedAt == 0 || x.DeletedAt == null))
                                    ?? throw new NotFoundException("NOT_FOUND");

        return entity;
    }

    public async Task<PaginationModel<T>> GetAllByParametersPaginatedAsync(
        PaginationQuery query,
        Expression<Func<T, bool>> whereExpression,
        params string[] includes)
    {
        var queryable = _context.Set<T>()
                                .AsNoTracking()
                                .ApplyIncludes(includes)
                                .Where(whereExpression)
                                .Where(x => x.DeletedAt == 0 || x.DeletedAt == null);

        return await PaginateAsync(queryable, query);
    }

    public async Task<List<T>> GetAllByParametersAsync(
        Expression<Func<T, bool>> whereExpression,
        params string[] includes)
    {
        return await _context.Set<T>()
                            .AsNoTracking()
                            .ApplyIncludes(includes)
                            .Where(whereExpression)
                            .Where(x => x.DeletedAt == 0 || x.DeletedAt == null)
                            .ToListAsync();
    }

    public async Task<T> GetByParametersAsync(
        Expression<Func<T, bool>> whereExpression,
        params string[] includes)
    {
        var entity = await _context.Set<T>()
                                    .AsNoTracking()
                                    .ApplyIncludes(includes)
                                    .Where(x => x.DeletedAt == 0 || x.DeletedAt == null)
                                    .FirstOrDefaultAsync(whereExpression)
                                    ?? throw new NotFoundException("NOT_FOUND");

        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var existingEntity = await _context.Set<T>()
                                            .FirstOrDefaultAsync(x => x.Id == entity.Id && (x.DeletedAt == 0 || x.DeletedAt == null))
                                            ?? throw new NotFoundException("NOT_FOUND");

        entity.UpdatedAt = DateHelper.GetCurrentTimestamp();
        _context.Entry(existingEntity).CurrentValues.SetValues(entity);

        await _context.SaveChangesAsync();

        return entity;
    }

    /// <summary>
    /// Manual pagination implementation for test purposes.
    /// </summary>
    private static async Task<PaginationModel<T>> PaginateAsync(IQueryable<T> queryable, PaginationQuery query)
    {
        var totalItems = await queryable.CountAsync();
        var items = await queryable
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .ToListAsync();

        return new PaginationModel<T>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = query.Page,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.Limit)
        };
    }
}

/// <summary>
/// Extension methods for applying includes to queryables.
/// </summary>
public static class QueryableExtensions
{
    public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query, params string[] includes) where T : class
    {
        if (includes != null)
        {
            foreach (var include in includes)
            {
                if (!string.IsNullOrWhiteSpace(include))
                {
                    query = query.Include(include);
                }
            }
        }
        return query;
    }
}
