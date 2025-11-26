namespace Miccore.Clean.Sample.Api.Features.Samples.Common;

/// <summary>
/// Lazy-initialized AutoMapper instance for Sample endpoints.
/// Uses the unified SampleEndpointMapperProfile for all mappings.
/// </summary>
public static class SampleEndpointMapper
{
    private static readonly Lazy<AutoMapper.IMapper> Lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsAssembly);
            
            // Single unified profile for all Sample endpoint mappings
            cfg.AddProfile<SampleEndpointMapperProfile>();
        });
        return config.CreateMapper();
    });
    
    public static AutoMapper.IMapper Mapper => Lazy.Value;
}