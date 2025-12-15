namespace Miccore.Clean.Sample.Infrastructure.Repositories;

/// <summary>
/// Sample repository
/// </summary>
/// <param name="context">The database context.</param>
public class SampleRepository(SampleApplicationDbContext context) : BaseRepository<SampleEntity>(context), ISampleRepository
{
    // Add any additional methods specific to SampleRepository here
}