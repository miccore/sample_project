using Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample;
using Miccore.Clean.Sample.Api.Sample.Mappers;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample
{
    /// <summary>
    /// Endpoint for creating a sample.
    /// </summary>
    public sealed class UpdateSampleEndpoint (IMediator _mediator) : Endpoint<UpdateSampleRequest, ApiResponse<UpdateSampleResponse>>
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
            Put(_route);
            AllowAnonymous();
        }
        
        /// <summary>
        /// Handles the request asynchronously.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override async Task HandleAsync(UpdateSampleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (ValidationFailed)
                {   var failures = ValidationFailures.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList();
                    throw new ValidatorException(string.Join("\n", failures));
                }
                
                 // Send the request to the mediator and wait for the response
                var response = await _mediator.Send(Mapper.Map<UpdateSampleCommand>(request), cancellationToken);
                
                await SendAsync(ApiResponse<UpdateSampleResponse>.Success(
                        Mapper.Map<UpdateSampleResponse>(response)
                    ), cancellation: cancellationToken);
            }
            // not found exception
            catch (NotFoundException notFound)
            {
                await SendAsync(ApiResponse<UpdateSampleResponse>.Error(HttpStatusCode.NotFound, notFound.Message), (int) HttpStatusCode.NotFound, cancellationToken);
            }
            // invalid data validation exception
            catch (ValidatorException invalid)
            {
                await SendAsync(ApiResponse<UpdateSampleResponse>.Error(HttpStatusCode.BadRequest, invalid.Message), (int) HttpStatusCode.BadRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                await SendAsync(ApiResponse<UpdateSampleResponse>.Error(HttpStatusCode.InternalServerError, ex.Message), (int) HttpStatusCode.InternalServerError, cancellationToken);
            }
        }
    }
}