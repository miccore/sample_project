using Miccore.Clean.Sample.Application.Sample.Responses;

namespace Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample
{
    /// <summary>
    /// Sample Command request
    /// </summary>
    public record UpdateSampleCommand : IRequest<SampleResponse>
    {
        /// <summary>
        /// Gets or sets the ID of the sample
        /// </summary>
        public Guid Id
        {
            get;
            set;
        }

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