using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.SampleFolder.Queries.GetAllSamples
{
    public sealed class GetAllSamplesQueryHandler : BaseQueryHandler<GetAllSamplesQuery, PaginationModel<SampleResponse>>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper;

        public GetAllSamplesQueryHandler(
            ISampleRepository sampleRepository,
            IMapper mapper,
            ILogger<GetAllSamplesQueryHandler> logger)
        {
            _sampleRepository = sampleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Handle for getting all sample paginated or not
        /// </summary>
        protected override async Task<PaginationModel<SampleResponse>> HandleQuery(GetAllSamplesQuery request, CancellationToken cancellationToken)
        {
            // get items
            var entities = await _sampleRepository.GetAllAsync(request.Query);
            
            // mapping with response
            return _mapper.Map<PaginationModel<SampleResponse>>(entities);
        }
    }
}