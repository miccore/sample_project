using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Commands.CreateSample
{
    public sealed class CreateSampleCommandHandler(ISampleRepository sampleRepository) : IRequestHandler<CreateSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository = sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        /// <summary>
        /// Sample command Handle for adding new element
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SampleResponse> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
        {
            // map request with the entity
            var sampleEntity = _mapper.Map<SampleEntity>(request) ?? throw new ApplicationException(ExceptionEnum.MapperIssue.GetEnumDescription());

            // add async with the repository
            var addedSample = await _sampleRepository.AddAsync(sampleEntity);

            //map with the response
            var sampleResponse = _mapper.Map<SampleResponse>(addedSample);

            // return response
            return sampleResponse;
        }
    }
}