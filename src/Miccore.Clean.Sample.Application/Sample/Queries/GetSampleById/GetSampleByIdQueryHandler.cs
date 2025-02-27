using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Sample.Mappers;

namespace Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById
{
    public sealed class GetSampleByIdQueryHandler(ISampleRepository sampleRepository) : IRequestHandler<GetSampleByIdQuery, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository = sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        /// <summary>
        /// getting sample by id query
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SampleResponse> Handle(GetSampleByIdQuery request, CancellationToken cancellationToken)
        {
            // get entity by id
            var entity = await _sampleRepository.GetByIdAsync(request.Id);
      
            // mapping response
            var response = _mapper.Map<SampleResponse>(entity);

            // return object
            return response;
        }
    }
}