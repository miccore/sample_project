namespace Miccore.Clean.Sample.Api.Features.Samples.CreateSample
{
    public class CreateSampleRequest
    {
        [Required]
        public string? Name { get; set; }
    }
}