namespace Miccore.Clean.Sample.Core.Configurations;

/// <summary>
/// Database connection configuration model.
/// Maps to the "Database" section in appsettings.json.
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// The section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Database";

    /// <summary>
    /// The database server hostname or IP address.
    /// </summary>
    public string Server { get; set; } = string.Empty;

    /// <summary>
    /// The database server port.
    /// </summary>
    public int Port { get; set; } = 3306;

    /// <summary>
    /// The database name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The database user.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// The database password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Builds the MySQL connection string from the configuration properties.
    /// </summary>
    /// <returns>The MySQL connection string.</returns>
    public string GetConnectionString()
    {
        return $"server={Server};port={Port};database={Name};user={User};password={Password}";
    }
}
