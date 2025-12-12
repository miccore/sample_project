using Miccore.Clean.Sample.Api.Features.Samples.CreateSample;
using Miccore.Clean.Sample.Api.Features.Samples.GetSample;
using Miccore.Clean.Sample.Api.Features.Samples.UpdateSample;
using Miccore.Clean.Sample.Api.Features.Samples.DeleteSample;
using Miccore.Clean.Sample.Api.Features.Samples.GetAllSamples;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;

namespace Miccore.Clean.Sample.Api.Features.Samples.Common;

/// <summary>
/// Unified AutoMapper profile for all Sample endpoint mappings.
/// Consolidates all request-to-command/query and response mappings in one place.
/// </summary>
public class SampleEndpointMapperProfile : Profile
{
    public SampleEndpointMapperProfile()
    {
        // === Response Mappings ===
        // Map between API model and Application response
        CreateMap<SampleModel, SampleResponse>().ReverseMap();

        // Pagination mapping
        CreateMap<PaginationModel<SampleModel>, PaginationModel<SampleResponse>>().ReverseMap();

        // === Request to Command/Query Mappings ===
        // Create
        CreateMap<CreateSampleRequest, CreateSampleCommand>();

        // Update
        CreateMap<UpdateSampleRequest, UpdateSampleCommand>();

        // Delete
        CreateMap<DeleteSampleRequest, DeleteSampleCommand>();

        // Get by Id
        CreateMap<GetSampleRequest, GetSampleQuery>();

        // Get All (inherits from PaginationQuery, no mapping needed)
    }
}