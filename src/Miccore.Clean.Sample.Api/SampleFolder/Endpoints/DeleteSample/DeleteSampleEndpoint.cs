using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample;
using Miccore.Clean.Sample.Api.Endpoints.Base;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.DeleteSample
{
    /// <summary>
    /// Endpoint for deleting a sample.
    /// </summary>
    public sealed class DeleteSampleEndpoint (IMediator _mediator) : BaseEndpoint<DeleteSampleRequest>
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
}