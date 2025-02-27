using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Commands.CreateSample
{
    /// <summary>
    /// Command to create a new sample
    /// </summary>
    public record CreateSampleCommand : IRequest<SampleResponse>
    {
        /// <summary>
        /// Gets or sets the name of the sample
        /// </summary>
        public string? Name
        {
            get;
            set;
        }
    }
}