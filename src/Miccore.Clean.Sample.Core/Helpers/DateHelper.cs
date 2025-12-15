namespace Miccore.Clean.Sample.Core.Helpers;

public static class DateHelper
{
    /// <summary>
    /// Gets the current timestamp in Unix time seconds.
    /// </summary>
    /// <returns>The current timestamp in Unix time seconds.</returns>
    public static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}