using System.ComponentModel;
using FluentAssertions;
using Miccore.Clean.Sample.Core.Extensions;

namespace Miccore.Clean.Sample.Core.Tests.Extensions;

public class EnumExtensionTests
{
    private enum SampleEnum
    {
        [Description("First Value Description")]
        FirstValue,
        SecondValue,
        [Description("Third Value Description")]
        ThirdValue
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnDescription_WhenDescriptionAttributeIsPresent()
    {
        // Arrange
        var enumValue = SampleEnum.FirstValue;

        // Act
        var description = enumValue.GetEnumDescription();

        // Assert
        description.Should().Be("First Value Description");
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnEnumValueAsString_WhenDescriptionAttributeIsNotPresent()
    {
        // Arrange
        var enumValue = SampleEnum.SecondValue;

        // Act
        var description = enumValue.GetEnumDescription();

        // Assert
        description.Should().Be("SecondValue");
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnDescription_WhenDescriptionAttributeIsPresentForThirdValue()
    {
        // Arrange
        var enumValue = SampleEnum.ThirdValue;

        // Act
        var description = enumValue.GetEnumDescription();

        // Assert
        description.Should().Be("Third Value Description");
    }
}