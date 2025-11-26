using AutoMapper;
using Miccore.Clean.Sample.Application.Features.Samples.Mappers;

namespace Miccore.Clean.Sample.Application.Tests.Fixtures;

/// <summary>
/// Factory for creating AutoMapper instances in tests.
/// </summary>
public static class TestMapperFactory
{
    private static readonly Lazy<IMapper> LazyMapper = new(() =>
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SampleMappingProfile>();
        });
        config.AssertConfigurationIsValid();
        return config.CreateMapper();
    });

    public static IMapper Create() => LazyMapper.Value;
}
