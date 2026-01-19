namespace Miccore.Clean.Sample.Api.Helpers;

/// <summary>
/// Helper class for extracting feature information from endpoint types.
/// </summary>
public static class FeatureHelper
{
    /// <summary>
    /// Extracts the feature group name from a type's namespace.
    /// </summary>
    /// <param name="type">The type to extract the feature from.</param>
    /// <returns>The feature group name, or "General" if no feature is found.</returns>
    public static string GetFeatureGroup(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var namespaceParts = type.Namespace?.Split('.') ?? [];
        var featureIndex = Array.IndexOf(namespaceParts, "Features");

        return featureIndex >= 0 && featureIndex < namespaceParts.Length - 1
            ? namespaceParts[featureIndex + 1]
            : "General";
    }
}
