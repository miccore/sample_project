using FluentValidation.TestHelper;
using Miccore.Clean.Sample.Api.Features.Samples.CreateSample;
using Miccore.Clean.Sample.Core.Enums;
using Miccore.Clean.Sample.Core.Extensions;

namespace Miccore.Clean.Sample.Api.Tests.Sample.CreateSample;

public class CreateSampleValidatorTests
{
    private readonly CreateSampleValidator _validator;

    public CreateSampleValidatorTests()
    {
        _validator = new CreateSampleValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Null()
    {
        // Arrange
        var request = new CreateSampleRequest { Name = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ValidatorEnum.NotNull.GetEnumDescription());
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var request = new CreateSampleRequest { Name = "" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ValidatorEnum.NotEmpty.GetEnumDescription());
    }

    [Fact]
    public void Should_Not_Have_Error_When_Name_Is_Provided()
    {
        // Arrange
        var request = new CreateSampleRequest { Name = "Valid Name" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
