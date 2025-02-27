using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample
{
    public record DeleteSampleCommand(Guid Id) : IRequest<SampleResponse>;
}