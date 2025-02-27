using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples
{
    /// <summary>
    /// Sample query model
    /// </summary>
    public record GetAllSamplesQuery(PaginationQuery Query) : IRequest<PaginationModel<SampleResponse>>;
}