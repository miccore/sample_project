using FluentValidation.TestHelper;
using Miccore.Clean.Sample.Api.Features.Samples.UpdateSample;
using Miccore.Clean.Sample.Core.Enums;
using Miccore.Clean.Sample.Core.Extensions;

namespace Miccore.Clean.Sample.Api.Tests.Sample.UpdateSample;

public class UpdateSampleValidatorTests
{
    private readonly UpdateSampleValidator _validator;

    public UpdateSampleValidatorTests()
    {
        _validator = new UpdateSampleValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Null()
    {
        // Arrange
        var request = new UpdateSampleRequest { Name = null };

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
        var request = new UpdateSampleRequest { Name = string.Empty };

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
        var request = new UpdateSampleRequest { Name = "Valid Name" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}