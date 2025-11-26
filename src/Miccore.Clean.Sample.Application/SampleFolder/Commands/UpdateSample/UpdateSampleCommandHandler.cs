using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample
{
    /// <summary>
    /// Update Sample Command Handler 
    /// </summary>
    public sealed class UpdateSampleCommandHandler : BaseCommandHandler<UpdateSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        public UpdateSampleCommandHandler(
            ISampleRepository sampleRepository,
            ILogger<UpdateSampleCommandHandler> logger) : base(logger)
        {
            _sampleRepository = sampleRepository;
        }

        /// <summary>
        /// Sample command Handle for updating element
        /// </summary>
        protected override async Task<SampleResponse> HandleCommand(UpdateSampleCommand request, CancellationToken cancellationToken)
        {
            // map request with the entity
            var sampleEntity = _mapper.Map<SampleEntity>(request) 
                ?? throw new ApplicationException(ExceptionEnum.MapperIssue.GetEnumDescription());

            // update with the repository
            var updatedSample = await _sampleRepository.UpdateAsync(sampleEntity);

            // map with the response
            return _mapper.Map<SampleResponse>(updatedSample);
        }
    }
}