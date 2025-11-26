using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;

namespace Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample;

public sealed class CreateSampleCommandHandler(ISampleRepository sampleRepository, IMapper mapper) 
    : BaseCommandHandler<CreateSampleCommand, SampleResponse>
{
    private readonly ISampleRepository _sampleRepository = sampleRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Sample command Handle for adding new element
    /// </summary>
    protected override async Task<SampleResponse> HandleCommand(CreateSampleCommand request, CancellationToken cancellationToken)
    {
        // map request with the entity
        var sampleEntity = _mapper.Map<SampleEntity>(request) 
            ?? throw new ApplicationException(ExceptionEnum.MapperIssue.GetEnumDescription());

        // add async with the repository
        var addedSample = await _sampleRepository.AddAsync(sampleEntity);

        // map with the response
        return _mapper.Map<SampleResponse>(addedSample);
    }
}