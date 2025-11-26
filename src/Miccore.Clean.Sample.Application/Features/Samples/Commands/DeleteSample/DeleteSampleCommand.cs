using Miccore.Clean.Sample.Application.Features.Samples.Responses;

namespace Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample
{
    public record DeleteSampleCommand(Guid Id) : IRequest<SampleResponse>;
}