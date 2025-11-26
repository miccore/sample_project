using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample
{
    public sealed class DeleteSampleCommandHandler : BaseCommandHandler<DeleteSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        public DeleteSampleCommandHandler(
            ISampleRepository sampleRepository,
            ILogger<DeleteSampleCommandHandler> logger) : base(logger)
        {
            _sampleRepository = sampleRepository;
        }

        /// <summary>
        /// Handles the delete sample command.
        /// </summary>
        protected override async Task<SampleResponse> HandleCommand(DeleteSampleCommand request, CancellationToken cancellationToken)
        {
            // delete with the repository
            var deletedSample = await _sampleRepository.DeleteAsync(request.Id);

            // map with the response
            return _mapper.Map<SampleResponse>(deletedSample);
        }
    }
}