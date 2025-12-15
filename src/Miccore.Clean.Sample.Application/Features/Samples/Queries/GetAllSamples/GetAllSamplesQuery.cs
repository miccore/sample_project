using Miccore.Clean.Sample.Application.Features.Samples.Responses;

namespace Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;

/// <summary>
/// Sample query model
/// </summary>
public record GetAllSamplesQuery(PaginationQuery Query) : IRequest<PaginationModel<SampleResponse>>;