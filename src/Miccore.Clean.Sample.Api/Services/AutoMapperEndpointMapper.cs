namespace Miccore.Clean.Sample.Api.Services;

/// <summary>
/// AutoMapper implementation of IEndpointMapper.
/// </summary>
public class AutoMapperEndpointMapper : IEndpointMapper
{
    private readonly AutoMapper.IMapper _mapper;

    public AutoMapperEndpointMapper(AutoMapper.IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TSource, TDestination>(source);
    }
}
