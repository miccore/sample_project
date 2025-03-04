namespace Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample
{
    public class CreateSampleValidator : Validator<CreateSampleRequest>
    {
        public CreateSampleValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidatorEnum.NotEmpty.GetEnumDescription())
            .NotNull()
            .WithMessage(ValidatorEnum.NotNull.GetEnumDescription());
        }
    }
}