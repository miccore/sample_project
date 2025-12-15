using FluentAssertions;
using Miccore.Clean.Sample.Core.Exceptions;

namespace Miccore.Clean.Sample.Core.Tests.Exceptions;

public class ValidatorExceptionTests
{
    [Fact]
    public void ValidatorException_ShouldSetMessage_WhenMessageIsProvided()
    {
        // Arrange
        var message = "Validation failed";

        // Act
        var exception = new ValidatorException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void ValidatorException_ShouldSetMessageAndInnerException_WhenMessageAndInnerExceptionAreProvided()
    {
        // Arrange
        var message = "Validation failed";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new ValidatorException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void ValidatorException_ShouldSetMessageFromInnerException_WhenInnerExceptionIsProvided()
    {
        // Arrange
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new ValidatorException(innerException);

        // Assert
        exception.Message.Should().Be(innerException.Message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void ValidatorException_ShouldSetDefaultMessage_WhenNoArgumentsAreProvided()
    {
        // Act
        var exception = new ValidatorException();

        // Assert
        exception.Message.Should().Be("Exception of type 'Miccore.Clean.Sample.Core.Exceptions.ValidatorException' was thrown.");
    }
}