using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById
{
    public record GetSampleByIdQuery(Guid Id) : IRequest<SampleResponse>;
}