using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;

namespace Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample;

public sealed class DeleteSampleCommandHandler(ISampleRepository sampleRepository, IMapper mapper) 
    : BaseCommandHandler<DeleteSampleCommand, SampleResponse>
{
    private readonly ISampleRepository _sampleRepository = sampleRepository;
    private readonly IMapper _mapper = mapper;

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