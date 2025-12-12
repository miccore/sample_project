using Miccore.Clean.Sample.Core.Configurations;

namespace Miccore.Clean.Sample.Infrastructure.Persistance;

/// <summary>
/// Database context registration
/// </summary>
public class SampleApplicationDbContext(DbContextOptions<SampleApplicationDbContext> options, IConfiguration configuration) : DbContext(options)
{
    // Configuration object to access app settings
    private readonly IConfiguration _configuration = configuration;

    #region DbSet
    public virtual DbSet<SampleEntity> Samples { get; set; }
    #endregion

    // Method to configure the database connection
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Get database configuration from appsettings using strongly-typed pattern
        var dbConfig = new DatabaseConfiguration();
        _configuration.GetSection(DatabaseConfiguration.SectionName).Bind(dbConfig);
        if (string.IsNullOrEmpty(dbConfig.Server))
            throw new InvalidOperationException($"Configuration section '{DatabaseConfiguration.SectionName}' is missing or invalid.");

        // Configure DbContext to use MySQL with Pomelo provider
        // ServerVersion.AutoDetect will automatically detect the MySQL/MariaDB version
        var connectionString = dbConfig.GetConnectionString();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    /// <summary>
    /// Configures the model with global query filters for soft delete.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global soft delete filter to all entities inheriting from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var deletedAtProperty = Expression.Property(parameter, nameof(BaseEntity.DeletedAt));

                // Filter: DeletedAt == 0 || DeletedAt == null
                var zero = Expression.Constant((long?)0, typeof(long?));
                var nullValue = Expression.Constant(null, typeof(long?));

                var isZero = Expression.Equal(deletedAtProperty, zero);
                var isNull = Expression.Equal(deletedAtProperty, nullValue);
                var filter = Expression.OrElse(isZero, isNull);

                var lambda = Expression.Lambda(filter, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}