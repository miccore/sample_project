using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById;
using Miccore.Clean.Sample.Api.Endpoints.Base;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.GetSampleById;

/// <summary>
/// Endpoint for getting a sample by its ID.
/// </summary>
public sealed class GetSampleByIdEndpoint(IMediator _mediator) : BaseEndpoint<GetSampleByIdRequest, GetSampleByIdSampleResponse>
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
        
        await SendSuccessAsync(Mapper.Map<GetSampleByIdSampleResponse>(response), cancellationToken);
    }
}
