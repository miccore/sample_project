namespace Miccore.Clean.Sample.Core.ApiModels
{
    /// <summary>
    /// Api error model
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string? Message { get; set; }
        
    }
}