namespace Miccore.Clean.Sample.Core.Enums;

/// <summary>
/// Enum for validators
/// </summary>
public enum ValidatorEnum
{
    [Description("NOT_EMPTY")]
    NotEmpty,
    [Description("NOT_NULL")]
    NotNull,
    [Description("NOT_NULL_OR_EMPTY")]
    NotNullOrEmpty,
}