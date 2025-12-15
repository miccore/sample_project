using Miccore.Clean.Sample.Api.Endpoints;
using Miccore.Clean.Sample.Api.Features.Samples.Common;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample;

namespace Miccore.Clean.Sample.Api.Features.Samples.DeleteSample;

/// <summary>
/// Endpoint for deleting a sample.
/// </summary>
public sealed class DeleteSampleEndpoint(IMediator _mediator) : BaseEndpoint<DeleteSampleRequest>
{
    private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;

    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete(BuildRoute("samples/{id}"));
        AllowAnonymous();
    }

    /// <summary>
    /// Executes the endpoint logic.
    /// </summary>
    protected override async Task ExecuteAsync(DeleteSampleRequest request, CancellationToken cancellationToken)
    {
        var command = Mapper.Map<DeleteSampleCommand>(request);
        await _mediator.Send(command, cancellationToken);

        await SendNoContentAsync(cancellationToken);
    }
}