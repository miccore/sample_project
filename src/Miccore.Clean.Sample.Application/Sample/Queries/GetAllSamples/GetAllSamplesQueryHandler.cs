using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples
{
    public sealed class GetAllSamplesQueryHandler(ISampleRepository sampleRepository) : IRequestHandler<GetAllSamplesQuery, PaginationModel<SampleResponse>>
    {
        private readonly ISampleRepository _sampleRepository = sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        /// <summary>
        /// handle for getting all sample paginated or not
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PaginationModel<SampleResponse>> Handle(GetAllSamplesQuery request, CancellationToken cancellationToken)
        {
            // get items
            var entities =  await _sampleRepository.GetAllAsync(request.Query);
            
            // mapping with response
            var responses = _mapper.Map<PaginationModel<SampleResponse>>(entities);

            // return response
            return responses;
        }
    }
}