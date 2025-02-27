using Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.GetAllSamples
{
    public class GetAllSamplesMapper : Profile
    {
        public GetAllSamplesMapper()
        {
            // Mapping between GetAllSamplesRequest and GetAllSamplesQuery
            CreateMap<GetAllSamplesRequest, GetAllSamplesQuery>().ReverseMap();
            // Mapping between GetAllSamplesResponse and SampleResponse
            CreateMap<GetAllSamplesResponse, SampleResponse>().ReverseMap();
            // Mapping between GetAllSamplesResponse and SampleResponse
            CreateMap<PaginationModel<GetAllSamplesResponse>, PaginationModel<SampleResponse>>().ReverseMap();
        }
    }
}