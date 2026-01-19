namespace Miccore.Clean.Sample.Api.Configuration;

/// <summary>
/// Configuration options for API endpoints.
/// </summary>
public class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>
    /// The route prefix for all API endpoints (e.g., "api").
    /// </summary>
    public string RoutePrefix { get; set; } = "api";

    /// <summary>
    /// The default API version (e.g., "v1").
    /// </summary>
    public string DefaultVersion { get; set; } = "v1";

    /// <summary>
    /// Whether to enable Swagger UI.
    /// </summary>
    public bool EnableSwagger { get; set; } = true;

    /// <summary>
    /// Swagger document title.
    /// </summary>
    public string SwaggerTitle { get; set; } = "Miccore Clean Sample API";

    /// <summary>
    /// Swagger document description.
    /// </summary>
    public string SwaggerDescription { get; set; } = "API for managing samples";
}
