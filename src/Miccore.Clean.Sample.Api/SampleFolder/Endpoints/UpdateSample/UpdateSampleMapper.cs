using Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample
{
    public class UpdateSampleMapper : Profile
    {
        public UpdateSampleMapper()
        {
            // Mapping between UpdateSampleRequest and UpdateSampleCommand
            CreateMap<UpdateSampleRequest, UpdateSampleCommand>().ReverseMap();   
            // Mapping between UpdateSampleResponse and UpdateSampleResponse
            CreateMap<UpdateSampleResponse, SampleResponse>().ReverseMap();
        }
    }
}