namespace Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample
{
    public class CreateSampleRequest
    {
        [Required]
        public string? Name { get; set; }
    }
}