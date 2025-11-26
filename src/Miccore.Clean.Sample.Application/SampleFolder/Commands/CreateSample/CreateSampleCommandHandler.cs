using Miccore.Clean.Sample.Application.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Application.Handlers;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.Sample.Commands.CreateSample
{
    public sealed class CreateSampleCommandHandler : BaseCommandHandler<CreateSampleCommand, SampleResponse>
    {
        private readonly ISampleRepository _sampleRepository;
        private readonly IMapper _mapper = SampleMapper.Mapper;

        public CreateSampleCommandHandler(
            ISampleRepository sampleRepository,
            ILogger<CreateSampleCommandHandler> logger) : base(logger)
        {
            _sampleRepository = sampleRepository;
        }

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
}