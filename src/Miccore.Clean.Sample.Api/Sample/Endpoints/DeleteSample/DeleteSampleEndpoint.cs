using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.DeleteSample
{
    public sealed class DeleteSampleEndpoint (IMediator _mediator) : Endpoint<DeleteSampleRequest>
    {
        // add auto mapper
        private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;
        // add route
        private readonly string _route = "sample/{id}";

        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        public override void Configure()
        {
            Delete(_route);
            AllowAnonymous();
        }
        
        /// <summary>
        /// Handles the request asynchronously.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override async Task HandleAsync(DeleteSampleRequest request, CancellationToken cancellationToken)
        {
            try {

                if (ValidationFailed)
                {   
                    var failures = ValidationFailures.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList();
                    throw new ValidatorException(string.Join("\n", failures));
                }

                // Send the request to the mediator and wait for the response
                 await _mediator.Send(Mapper.Map<DeleteSampleCommand>(request), cancellationToken);

                // return no content
                await SendNoContentAsync(cancellationToken);
            }
            // not found exception
            catch (NotFoundException notFound)
            {
                await SendAsync(ApiResponse<object>.Error(HttpStatusCode.NotFound, notFound.Message), (int) HttpStatusCode.NotFound, cancellationToken);
            }
            // invalid data validation exception
            catch (ValidatorException invalid)
            {
                await SendAsync(ApiResponse<object>.Error(HttpStatusCode.BadRequest, invalid.Message), (int) HttpStatusCode.BadRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                await SendAsync(ApiResponse<object>.Error(HttpStatusCode.InternalServerError, ex.Message), (int) HttpStatusCode.InternalServerError, cancellationToken);
            }
        }
    }
}