namespace Miccore.Clean.Sample.Api.Features.Samples.CreateSample;

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