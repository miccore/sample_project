using Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample;
using Miccore.Clean.Sample.Api.Sample.Endpoints.DeleteSample;
using Miccore.Clean.Sample.Api.Sample.Endpoints.GetAllSamples;
using Miccore.Clean.Sample.Api.Sample.Endpoints.GetSampleById;
using Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample;

namespace Miccore.Clean.Sample.Api.Sample.Mappers
{
    public class SampleEndpointMapper
    {
        private static readonly Lazy<AutoMapper.IMapper> Lazy = new(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsAssembly);

                #region profiles

                // sample mapping profile
                cfg.AddProfile<SampleEndpointMapperProfile>();
                cfg.AddProfile<CreateSampleMapper>();
                cfg.AddProfile<DeleteSampleMapper>();
                cfg.AddProfile<UpdateSampleMapper>();
                cfg.AddProfile<GetAllSamplesMapper>();
                cfg.AddProfile<GetSampleByIdMapper>();

                #endregion
            });
            var mapper = config.CreateMapper();
            return mapper;
        });
        public static AutoMapper.IMapper Mapper => Lazy.Value;
    }
}