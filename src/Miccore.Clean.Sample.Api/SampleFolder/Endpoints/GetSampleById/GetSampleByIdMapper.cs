using Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.GetSampleById;

public class GetSampleByIdMapper : Profile
{
    public GetSampleByIdMapper()
    {
        CreateMap<GetSampleByIdRequest, GetSampleByIdQuery>().ReverseMap();
        CreateMap<GetSampleByIdSampleResponse, SampleResponse>().ReverseMap();
    }
}
