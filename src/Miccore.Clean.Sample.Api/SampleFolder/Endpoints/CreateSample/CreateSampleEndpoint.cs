using Miccore.Clean.Sample.Application.Sample.Commands.CreateSample;
using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Api.Endpoints.Base;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample
{
    /// <summary>
    /// Endpoint for creating a sample.
    /// </summary>
    public class CreateSampleEndpoint (IMediator _mediator) : BaseEndpoint<CreateSampleRequest, CreateSampleResponse>
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
            
            await SendCreatedAsync(Mapper.Map<CreateSampleResponse>(response), cancellationToken);
        }
    }
}