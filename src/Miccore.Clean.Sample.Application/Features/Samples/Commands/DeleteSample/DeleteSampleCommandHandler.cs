using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;

namespace Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample;

public sealed class DeleteSampleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : BaseCommandHandler<DeleteSampleCommand, SampleResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handles the delete sample command.
    /// </summary>
    protected override async Task<SampleResponse> HandleCommand(DeleteSampleCommand request, CancellationToken cancellationToken)
    {
        // delete with the repository
        var deletedSample = await _unitOfWork.Samples.DeleteAsync(request.Id);

        // save changes via unit of work
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // map with the response
        return _mapper.Map<SampleResponse>(deletedSample);
    }
}