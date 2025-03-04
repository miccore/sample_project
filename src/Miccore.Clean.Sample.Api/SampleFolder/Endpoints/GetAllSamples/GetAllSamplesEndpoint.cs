using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.GetAllSamples
{
    public class GetAllSamplesEndpoint (IMediator _mediator) : Endpoint<GetAllSamplesRequest, ApiResponse<PaginationModel<GetAllSamplesResponse>>>
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
            Get(_route);
            AllowAnonymous();
        }
        
        /// <summary>
        /// Handles the request asynchronously.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public override async Task HandleAsync(GetAllSamplesRequest request, CancellationToken cancellationToken)
        {
            try
            {   
                // Send the request to the mediator and wait for the response
                var response = await _mediator.Send(new GetAllSamplesQuery(request), cancellationToken);

                // return date if not paginate
                if(!request.paginate)
                    await SendAsync(ApiResponse<PaginationModel<GetAllSamplesResponse>>.Success(
                        Mapper.Map<PaginationModel<GetAllSamplesResponse>>(response)
                    ), cancellation: cancellationToken);

                // add next and previous links
                response.AddRouteLink("", request);

                await SendAsync(ApiResponse<PaginationModel<GetAllSamplesResponse>>.Success(
                     Mapper.Map<PaginationModel<GetAllSamplesResponse>>(response)
                ), cancellation: cancellationToken);
            }
            catch (Exception ex)
            {
                await SendAsync(ApiResponse<PaginationModel<GetAllSamplesResponse>>.Error(HttpStatusCode.InternalServerError, ex.Message), (int) HttpStatusCode.InternalServerError, cancellationToken);
            }
        }
    }
}