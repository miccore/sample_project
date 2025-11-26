using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById
{
    public sealed class GetSampleByIdQueryHandler : BaseQueryHandler<GetSampleByIdQuery, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        public GetSampleByIdQueryHandler(
            ISampleRepository sampleRepository,
            ILogger<GetSampleByIdQueryHandler> logger) : base(logger)
        {
            _sampleRepository = sampleRepository;
        }

        /// <summary>
        /// Getting sample by id query
        /// </summary>
        protected override async Task<SampleResponse> HandleQuery(GetSampleByIdQuery request, CancellationToken cancellationToken)
        {
            // get entity by id
            var entity = await _sampleRepository.GetByIdAsync(request.Id);
      
            // mapping response
            return _mapper.Map<SampleResponse>(entity);
        }
    }
}