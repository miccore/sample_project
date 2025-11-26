using Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample;
using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Api.Endpoints.Base;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample
{
    /// <summary>
    /// Endpoint for updating a sample.
    /// </summary>
    public sealed class UpdateSampleEndpoint (IMediator _mediator) : BaseEndpoint<UpdateSampleRequest, UpdateSampleResponse>
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
            
            await SendSuccessAsync(Mapper.Map<UpdateSampleResponse>(response), cancellationToken);
        }
    }
}