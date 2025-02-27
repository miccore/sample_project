namespace Miccore.Clean.Sample.Application.Sample.Mappers
{
    /// <summary>
    /// General class for adding profiles
    /// </summary>
    public class SampleMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod != null && (p.GetMethod.IsPublic || p.GetMethod.IsAssembly);

                #region profiles

                // Add sample mapping profile
                cfg.AddProfile<SampleMappingProfile>();

                #endregion
            });
            var mapper = config.CreateMapper();
            return mapper;
        });
        public static IMapper Mapper => Lazy.Value;
    }
}