namespace Miccore.Clean.Sample.Core.Extensions;

public static class IQueryableExtension
{
    /// <summary>
    /// Applies the specified includes to the query.
    /// </summary>
    /// <param name="query">The query to apply includes to.</param>
    /// <param name="includes">The includes to apply.</param>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <returns>The query with the includes applied.</returns>
    public static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query, params string[] includes) where T : class
    {
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }
        return query;
    }
}