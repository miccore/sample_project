namespace Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample
{
    public class UpdateSampleRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string? Name { get; set; }
    }
}