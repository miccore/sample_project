namespace Miccore.Clean.Sample.Infrastructure.Persistence
{
    /// <summary>
    /// Database context registration
    /// </summary>
    public class SampleApplicationDbContext(DbContextOptions<SampleApplicationDbContext> options, IConfiguration configuration) : DbContext(options)
    {
        // Configuration object to access app settings
        private readonly IConfiguration configuration = configuration;

        #region DbSet
            public virtual DbSet<SampleEntity> Samples { get; set; }
        #endregion

        // Method to configure the database connection
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get connection string from environment file
            var connectionString = $"server={configuration["Server"]};port={configuration["Port"]};database={configuration["Database"]};user={configuration["User"]};password={configuration["Password"]}";

            // Configure DbContext to use MySQL with the connection string
            optionsBuilder.UseMySQL(connectionString);
        }
    }
}