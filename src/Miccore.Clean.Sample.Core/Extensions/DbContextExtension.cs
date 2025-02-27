namespace Miccore.Clean.Sample.Core.Extensions
{
    // Static helper class for DbContext operations
    public static class DbContextExtension
    {
        /// <summary>
        /// Sets the values for updating an entity.
        /// </summary>
        /// <param name="entity">The entity with new values.</param>
        /// <param name="context">The existing entity to update.</param>
        /// <returns>The updated entity.</returns>
        public static T SetUpdatedValues<T>(this T context, T entity) where T : BaseEntity
        {
            // Iterate through each property of the entity
            foreach (var property in entity.GetType().GetProperties())
            {
                // Get the value of the current property from the entity
                var prop = property.GetValue(entity);
                // Get the corresponding property from the context
                var con = context.GetType().GetProperty(property.Name);

                // Skip if the property value is null or con is null
                if(prop is null || con is null) continue;

                // Update the context property if the values are different
                if(prop != con.GetValue(context))
                    con.SetValue(context, prop);
            }

            // Return the updated context
            return context;
        }
    }
}