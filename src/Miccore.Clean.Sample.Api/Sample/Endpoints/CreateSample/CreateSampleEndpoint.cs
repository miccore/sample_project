using Miccore.Clean.Sample.Application.Sample.Commands.CreateSample;
using Miccore.Clean.Sample.Api.Sample.Mappers;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample
{
    /// <summary>
    /// Endpoint for creating a sample.
    /// </summary>
    public class CreateSampleEndpoint (IMediator _mediator) : Endpoint<CreateSampleRequest, ApiResponse<CreateSampleResponse>>
    {
        // add auto mapper
        private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;
        // add route
        private readonly string _route = "sample";

        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        public override void Configure()
        {
            Post(_route);
            AllowAnonymous();
        }
        
        /// <summary>
        /// Handles the request asynchronously.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override async Task HandleAsync(CreateSampleRequest request, CancellationToken cancellationToken)
        {
            try{
                if (ValidationFailed)
                {   
                    var failures = ValidationFailures.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList();
                    throw new ValidatorException(string.Join("\n", failures));
                }

                // Send the request to the mediator and wait for the response
                var response = await _mediator.Send(Mapper.Map<CreateSampleCommand>(request), cancellationToken);
                
                var map = ApiResponse<CreateSampleResponse>.Success(
                        Mapper.Map<CreateSampleResponse>(response)
                    );

                await SendAsync(ApiResponse<CreateSampleResponse>.Success(
                        Mapper.Map<CreateSampleResponse>(response)
                    ), (int) HttpStatusCode.Created
                , cancellationToken);
            }
            // not found exception
            catch (NotFoundException notFound)
            {
                await SendAsync(ApiResponse<CreateSampleResponse>.Error(HttpStatusCode.NotFound, notFound.Message), (int) HttpStatusCode.NotFound, cancellationToken);
            }
            // invalid data validation exception
            catch (ValidatorException invalid)
            {
                await SendAsync(ApiResponse<CreateSampleResponse>.Error(HttpStatusCode.BadRequest, invalid.Message), (int) HttpStatusCode.BadRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                await SendAsync(ApiResponse<CreateSampleResponse>.Error(HttpStatusCode.InternalServerError, ex.Message), (int) HttpStatusCode.InternalServerError, cancellationToken);
            }
        }
    }
}