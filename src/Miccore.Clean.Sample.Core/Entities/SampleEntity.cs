namespace Miccore.Clean.Sample.Core.Entities;

/// <summary>
/// Represents a sample entity.
/// </summary>
[Table("Sample")]
public class SampleEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the sample entity.
    /// </summary>
    public string? Name { get; set; }
}