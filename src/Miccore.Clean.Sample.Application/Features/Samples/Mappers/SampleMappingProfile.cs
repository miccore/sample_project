using Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;

namespace Miccore.Clean.Sample.Application.Features.Samples.Mappers;

/// <summary>
/// AutoMapper profile for Sample entity mappings.
/// </summary>
public class SampleMappingProfile : Profile
{
    public SampleMappingProfile()
    {
        // Entity <-> Response
        CreateMap<SampleEntity, SampleResponse>().ReverseMap();

        // Entity <-> Commands
        CreateMap<SampleEntity, CreateSampleCommand>().ReverseMap();
        CreateMap<SampleEntity, UpdateSampleCommand>().ReverseMap();

        // Pagination mappings
        CreateMap<PaginationModel<SampleEntity>, PaginationModel<SampleResponse>>().ReverseMap();
    }
}
