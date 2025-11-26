using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples;
using Miccore.Clean.Sample.Api.Endpoints.Base;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.GetAllSamples
{
    /// <summary>
    /// Endpoint for getting all samples with pagination support.
    /// </summary>
    public class GetAllSamplesEndpoint (IMediator _mediator) : BaseEndpoint<GetAllSamplesRequest, PaginationModel<GetAllSamplesResponse>>
    {
        private static AutoMapper.IMapper Mapper => SampleEndpointMapper.Mapper;

        /// <summary>
        /// Configures the endpoint.
        /// </summary>
        public override void Configure()
        {
            Get(BuildRoute("samples"));
            AllowAnonymous();
        }
        
        /// <summary>
        /// Executes the endpoint logic.
        /// </summary>
        protected override async Task ExecuteAsync(GetAllSamplesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllSamplesQuery(request);
            var response = await _mediator.Send(query, cancellationToken);

            // Add pagination links if paginated
            if (request.paginate)
            {
                response.AddRouteLink(BuildRoute("samples"), request);
            }

            await SendSuccessAsync(Mapper.Map<PaginationModel<GetAllSamplesResponse>>(response), cancellationToken);
        }
    }
}