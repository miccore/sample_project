using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples
{
    public sealed class GetAllSamplesQueryHandler : BaseQueryHandler<GetAllSamplesQuery, PaginationModel<SampleResponse>>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        public GetAllSamplesQueryHandler(
            ISampleRepository sampleRepository,
            ILogger<GetAllSamplesQueryHandler> logger) : base(logger)
        {
            _sampleRepository = sampleRepository;
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