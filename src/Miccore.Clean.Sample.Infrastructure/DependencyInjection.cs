namespace Miccore.Clean.Sample.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            // database connection
            services.AddDbContext<SampleApplicationDbContext>();

            // add repositories
            #region repositories
                services.TryAddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
                services.TryAddScoped<ISampleRepository, SampleRepository>();
            #endregion

            return services;
        }
    }
}