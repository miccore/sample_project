namespace Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample
{
    public class UpdateSampleValidator : Validator<UpdateSampleRequest>
    {
        public UpdateSampleValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidatorEnum.NotEmpty.GetEnumDescription())
            .NotNull()
            .WithMessage(ValidatorEnum.NotNull.GetEnumDescription());
        }
    }
}