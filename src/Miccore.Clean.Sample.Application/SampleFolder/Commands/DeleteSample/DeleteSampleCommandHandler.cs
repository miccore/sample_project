using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample
{
    public sealed class DeleteSampleCommandHandler(ISampleRepository sampleRepository) : IRequestHandler<DeleteSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository = sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        /// <summary>
        /// Handles the delete sample command.
        /// </summary>
        /// <param name="request">The delete sample command request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the sample response.</returns>
        public async Task<SampleResponse> Handle(DeleteSampleCommand request, CancellationToken cancellationToken)
        {
            // add async with the repository
            var deletedSample = await _sampleRepository.DeleteAsync(request.Id);

            // map with the response
            var sampleResponse = _mapper.Map<SampleResponse>(deletedSample);

            // return response
            return sampleResponse;
        }
    }
}