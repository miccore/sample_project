using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Miccore.Clean.Sample.Core.Repositories.Base;

namespace Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;

public sealed class GetSampleQueryHandler(IReadOnlyRepository<SampleEntity> sampleRepository, IMapper mapper)
    : BaseQueryHandler<GetSampleQuery, SampleResponse>
{
    private readonly IReadOnlyRepository<SampleEntity> _sampleRepository = sampleRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Getting sample by id query
    /// </summary>
    protected override async Task<SampleResponse> HandleQuery(GetSampleQuery request, CancellationToken cancellationToken)
    {
        // get entity by id
        var entity = await _sampleRepository.GetByIdAsync(request.Id);

        // mapping response
        return _mapper.Map<SampleResponse>(entity);
    }
}
