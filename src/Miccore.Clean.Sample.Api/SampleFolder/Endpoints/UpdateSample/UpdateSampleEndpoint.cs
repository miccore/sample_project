using Miccore.Clean.Sample.Application.Features.Samples.Commands.UpdateSample;
using Miccore.Clean.Sample.Api.Features.Samples.Common;
using Miccore.Clean.Sample.Api.Endpoints;

namespace Miccore.Clean.Sample.Api.SampleFolder.Endpoints.UpdateSample
{
    /// <summary>
    /// Endpoint for updating a sample.
    /// </summary>
    public sealed class UpdateSampleEndpoint (IMediator _mediator) : BaseEndpoint<UpdateSampleRequest, SampleModel>
    {
        private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;

        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        public override void Configure()
        {
            Put(BuildRoute("samples/{id}"));
            AllowAnonymous();
        }
        
        /// <summary>
        /// Executes the endpoint logic.
        /// </summary>
        protected override async Task ExecuteAsync(UpdateSampleRequest request, CancellationToken cancellationToken)
        {
            var command = Mapper.Map<UpdateSampleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);
            
            await SendSuccessAsync(Mapper.Map<SampleModel>(response), cancellationToken);
        }
    }
}