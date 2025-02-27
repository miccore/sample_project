using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Enums;
using Miccore.Clean.Sample.Core.Extensions;
using FluentAssertions;

namespace Miccore.Clean.Sample.Core.Tests.Exceptions
{
    public class NotFoundExceptionTests
    {
        [Fact]
        public void NotFoundException_ShouldSetMessage_WhenMessageIsProvided()
        {
            // Arrange
            var message = "Entity not found";

            // Act
            var exception = new NotFoundException(message);

            // Assert
            exception.Message.Should().Be(message);
        }

        [Fact]
        public void NotFoundException_ShouldSetMessageAndInnerException_WhenMessageAndInnerExceptionAreProvided()
        {
            // Arrange
            var message = "Entity not found";
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new NotFoundException(message, innerException);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void NotFoundException_ShouldSetMessageFromInnerException_WhenInnerExceptionIsProvided()
        {
            // Arrange
            var innerException = new Exception("Inner exception");

            // Act
            var exception = new NotFoundException(innerException);

            // Assert
            exception.Message.Should().Be(innerException.Message);
            exception.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void NotFoundException_ShouldSetDefaultMessage_WhenNoArgumentsAreProvided()
        {
            // Act
            var exception = new NotFoundException();

            // Assert
            exception.Message.Should().Be(ExceptionEnum.NotFound.GetEnumDescription());
        }
    }
}