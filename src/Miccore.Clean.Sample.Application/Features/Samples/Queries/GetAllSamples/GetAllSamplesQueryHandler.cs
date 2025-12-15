using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Miccore.Clean.Sample.Core.Repositories.Base;

namespace Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;

public sealed class GetAllSamplesQueryHandler(IReadOnlyRepository<SampleEntity> sampleRepository, IMapper mapper)
    : BaseQueryHandler<GetAllSamplesQuery, PaginationModel<SampleResponse>>
{
    private readonly IReadOnlyRepository<SampleEntity> _sampleRepository = sampleRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle for getting all sample paginated or not
    /// </summary>
    protected override async Task<PaginationModel<SampleResponse>> HandleQuery(GetAllSamplesQuery request, CancellationToken cancellationToken)
    {
        // get items
        var entities = await _sampleRepository.GetAllAsync(request.Query);

        // mapping with response
        return _mapper.Map<PaginationModel<SampleResponse>>(entities);
    }
}
