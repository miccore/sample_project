using Miccore.Clean.Sample.Application.Behaviors;
using Miccore.Clean.Sample.Application.Features.Samples.Mappers;

namespace Miccore.Clean.Sample.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Add application services including MediatR, AutoMapper, and pipeline behaviors.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register AutoMapper with all profiles from this assembly
        services.AddAutoMapper(typeof(SampleMappingProfile).Assembly);
        
        // Register MediatR with pipeline behaviors
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
            // Register pipeline behaviors in order of execution
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        return services;
    }
}