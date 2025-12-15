using Miccore.Clean.Sample.Application.Features.Samples.Responses;

namespace Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;

public record GetSampleQuery(Guid Id) : IRequest<SampleResponse>;