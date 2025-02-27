using Miccore.Clean.Sample.Api.Sample.Models;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Api.Sample.Mappers
{
    public class SampleEndpointMapperProfile : Profile
    {
        public SampleEndpointMapperProfile()
        {
            // Mapping between SampleModel and SampleResponse
            CreateMap<SampleModel, SampleResponse>().ReverseMap();
        }
    }
}