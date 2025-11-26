using Miccore.Clean.Sample.Application.Features.Samples.Responses;

namespace Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample
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