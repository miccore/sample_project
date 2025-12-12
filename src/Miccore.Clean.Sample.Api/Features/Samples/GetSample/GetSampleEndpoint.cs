using Miccore.Clean.Sample.Api.Features.Samples.Common;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;
using Miccore.Clean.Sample.Api.Endpoints;

namespace Miccore.Clean.Sample.Api.Features.Samples.GetSample;

/// <summary>
/// Endpoint for getting a sample by its ID.
/// </summary>
public sealed class GetSampleEndpoint(IMediator _mediator) : BaseEndpoint<GetSampleRequest, SampleModel>
{
    private static readonly AutoMapper.IMapper Mapper = SampleEndpointMapper.Mapper;

    public override void Configure()
    {
        Get(BuildRoute("samples/{id}"));
        AllowAnonymous();
    }

    /// <summary>
    /// Executes the endpoint logic.
    /// </summary>
    protected override async Task ExecuteAsync(GetSampleRequest request, CancellationToken cancellationToken)
    {
        var query = Mapper.Map<GetSampleQuery>(request);
        var response = await _mediator.Send(query, cancellationToken);

        await SendSuccessAsync(Mapper.Map<SampleModel>(response), cancellationToken);
    }
}
