namespace Miccore.Clean.Sample.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// add mediatr commands and query assembly
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            #region addMediatR
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            #endregion
            
            return services;
        }
    }
}