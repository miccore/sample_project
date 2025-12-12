using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.SampleFolder.Queries.GetSampleById
{
    public sealed class GetSampleByIdQueryHandler : BaseQueryHandler<GetSampleQuery, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper;

        public GetSampleByIdQueryHandler(
            ISampleRepository sampleRepository,
            IMapper mapper,
            ILogger<GetSampleByIdQueryHandler> logger)
        {
            _sampleRepository = sampleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Getting sample by id query
        /// </summary>
        protected override async Task<SampleResponse> HandleQuery(GetSampleQuery request, CancellationToken cancellationToken)
        {
            // get entity by id
            var entity = await _sampleRepository.GetByIdAsync(request.Id);

            // mapping response
            return _mapper.Map<SampleResponse>(entity);
        }
    }
}