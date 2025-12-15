using Miccore.Clean.Sample.Api.Endpoints;
using Miccore.Clean.Sample.Api.Features.Samples.Common;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;

namespace Miccore.Clean.Sample.Api.Features.Samples.GetAllSamples;

/// <summary>
/// Endpoint for getting all samples with pagination support.
/// </summary>
public class GetAllSamplesEndpoint(IMediator _mediator) : BaseEndpoint<GetAllSamplesRequest, PaginationModel<SampleModel>>
{
    private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;

    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get(BuildRoute("samples"));
        AllowAnonymous();
    }

    /// <summary>
    /// Executes the endpoint logic.
    /// </summary>
    protected override async Task ExecuteAsync(GetAllSamplesRequest request, CancellationToken cancellationToken)
    {
        var query = new GetAllSamplesQuery(request);
        var response = await _mediator.Send(query, cancellationToken);

        // Add pagination links if paginated
        if (request.Paginate)
        {
            response.AddRouteLink(BuildRoute("samples"), request);
        }

        await SendSuccessAsync(Mapper.Map<PaginationModel<SampleModel>>(response), cancellationToken);
    }
}