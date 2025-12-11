using Miccore.Clean.Sample.Api.Features.Samples.Common;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSampleById;
using Miccore.Clean.Sample.Api.Endpoints;

namespace Miccore.Clean.Sample.Api.SampleFolder.Endpoints.GetSampleById;

/// <summary>
/// Endpoint for getting a sample by its ID.
/// </summary>
public sealed class GetSampleByIdEndpoint(IMediator _mediator) : BaseEndpoint<GetSampleByIdRequest, SampleModel>
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
    protected override async Task ExecuteAsync(GetSampleByIdRequest request, CancellationToken cancellationToken)
    {
        var query = Mapper.Map<GetSampleByIdQuery>(request);
        var response = await _mediator.Send(query, cancellationToken);
        
        await SendSuccessAsync(Mapper.Map<SampleModel>(response), cancellationToken);
    }
}
