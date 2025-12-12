using Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample;
using Miccore.Clean.Sample.Api.Features.Samples.Common;
using Miccore.Clean.Sample.Api.Endpoints;

namespace Miccore.Clean.Sample.Api.Features.Samples.CreateSample;

/// <summary>
/// Endpoint for creating a sample.
/// </summary>
public class CreateSampleEndpoint(IMediator _mediator) : BaseEndpoint<CreateSampleRequest, SampleModel>
{
    private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;

    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post(BuildRoute("samples"));
        AllowAnonymous();
    }

    /// <summary>
    /// Executes the endpoint logic.
    /// </summary>
    protected override async Task ExecuteAsync(CreateSampleRequest request, CancellationToken cancellationToken)
    {
        var command = Mapper.Map<CreateSampleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        await SendCreatedAsync(Mapper.Map<SampleModel>(response), cancellationToken);
    }
}