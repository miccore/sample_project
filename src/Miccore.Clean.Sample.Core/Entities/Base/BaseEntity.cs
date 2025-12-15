using Miccore.Clean.Sample.Core.Helpers;

namespace Miccore.Clean.Sample.Core.Entities.Base;

/// <summary>
/// Base entity for all entities
/// </summary>
public abstract class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [DefaultValue(0)]
    public long CreatedAt { get; set; } = DateHelper.GetCurrentTimestamp();

    [DefaultValue(0)]
    public long? UpdatedAt { get; set; } = 0;

    [DefaultValue(0)]
    public long? DeletedAt { get; set; } = 0;
}