using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Sample.Mappers;

namespace Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample
{
    /// <summary>
    /// Update Sample Command Handler 
    /// </summary>
    public sealed class UpdateSampleCommandHandler(ISampleRepository sampleRepository) : IRequestHandler<UpdateSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository = sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        /// <summary>
        /// Sample command Handle for updating element
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SampleResponse> Handle(UpdateSampleCommand request, CancellationToken cancellationToken)
        {
            // map request with the entity
            var sampleEntity = _mapper.Map<Core.Entities.SampleEntity>(request) ?? throw new ApplicationException(ExceptionEnum.MapperIssue.GetEnumDescription());

            // add async with the repository
            var updatedSample = await _sampleRepository.UpdateAsync(sampleEntity);

            // map with the response
            var sampleResponse = _mapper.Map<SampleResponse>(updatedSample);

            // return response
            return sampleResponse;
        }
    }
}