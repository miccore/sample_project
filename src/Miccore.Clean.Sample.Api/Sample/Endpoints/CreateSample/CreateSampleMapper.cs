using Miccore.Clean.Sample.Application.Sample.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample
{
    public class CreateSampleMapper : Profile
    {
        public CreateSampleMapper()
        {
            // Mapping between CreateSampleRequest and CreateSampleCommand
            CreateMap<CreateSampleRequest, CreateSampleCommand>().ReverseMap();   
            // Mapping between CreateSampleResponse and CreateSampleResponse
            CreateMap<CreateSampleResponse, SampleResponse>().ReverseMap();
        }
    }
}