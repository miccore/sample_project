namespace Miccore.Clean.Sample.Core.Extensions;

public static class EnumExtension
{
    /// <summary>
    /// Gets the description attribute of an enum value.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="value">The enum value.</param>
    /// <returns>The description of the enum value, or the enum value as a string if no description is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public static string GetEnumDescription<T>(this T value) where T : Enum
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field == null)
        {
            return value.ToString();
        }

        DescriptionAttribute? attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute == null ? value.ToString() : attribute.Description;
    }
}