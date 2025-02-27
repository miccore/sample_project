using Miccore.Clean.Sample.Application.Sample.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Mappers
{
    /// <summary>
    /// Sample Mapping Profile map creation
    /// </summary>
    public class SampleMappingProfile : Profile
    {
        public SampleMappingProfile()
        {
            #region CreateMap

            // Map between SampleEntity and SampleResponse
            CreateMap<SampleEntity, SampleResponse>().ReverseMap();
            // Map between SampleEntity and CreateSampleCommand
            CreateMap<SampleEntity, CreateSampleCommand>().ReverseMap();
            // Map between SampleEntity and UpdateSampleCommand
            CreateMap<SampleEntity, UpdateSampleCommand>().ReverseMap();
            // Map between PaginationModel of SampleEntity and PaginationModel of SampleResponse
            CreateMap<PaginationModel<SampleEntity>, PaginationModel<SampleResponse>>().ReverseMap();

            #endregion
        }
    }
}