using Miccore.Clean.Sample.Application.Features.Samples.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.SampleFolder.Commands.UpdateSample
{
    /// <summary>
    /// Update Sample Command Handler 
    /// </summary>
    public sealed class UpdateSampleCommandHandler : BaseCommandHandler<UpdateSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper;

        public UpdateSampleCommandHandler(
            ISampleRepository sampleRepository,
            IMapper mapper,
            ILogger<UpdateSampleCommandHandler> logger)
        {
            _sampleRepository = sampleRepository;
            _mapper = mapper;
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