using Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.DeleteSample;

public class DeleteSampleMapper : Profile
{
    public DeleteSampleMapper()
    {
        CreateMap<DeleteSampleRequest, DeleteSampleCommand>().ReverseMap();
    }
}