using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Miccore.Clean.Sample.Application.Behaviors;
using Miccore.Clean.Sample.Core.Exceptions;
using Moq;

namespace Miccore.Clean.Sample.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenNoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "test" };
        var expectedResponse = new TestResponse { Id = 1 };
        var nextCalled = false;

        RequestHandlerDelegate<TestResponse> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "valid name" };
        var expectedResponse = new TestResponse { Id = 1 };
        var nextCalled = false;

        RequestHandlerDelegate<TestResponse> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidatorException()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Name is required")
        };

        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(new TestResponse { Id = 1 });

        // Act
        var act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidatorException>()
            .WithMessage("*Name is required*");
    }

    [Fact]
    public async Task Handle_WhenMultipleValidationErrorsExist_ShouldIncludeAllErrors()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        var validationFailures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Name", "Name must be at least 3 characters"),
            new("Email", "Email is invalid")
        };

        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(new TestResponse { Id = 1 });

        // Act
        var act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidatorException>();
        exception.Which.Message.Should().Contain("Name is required");
        exception.Which.Message.Should().Contain("Name must be at least 3 characters");
        exception.Which.Message.Should().Contain("Email is invalid");
    }

    [Fact]
    public async Task Handle_WhenMultipleValidatorsExist_ShouldRunAllValidators()
    {
        // Arrange
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Name", "Error from validator 1")
            }));

        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Email", "Error from validator 2")
            }));

        var validators = new List<IValidator<TestRequest>> { validator1Mock.Object, validator2Mock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(new TestResponse { Id = 1 });

        // Act
        var act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidatorException>();
        exception.Which.Message.Should().Contain("Error from validator 1");
        exception.Which.Message.Should().Contain("Error from validator 2");
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldNotCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Name", "Name is required")
            }));

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "" };
        var nextCalled = false;

        RequestHandlerDelegate<TestResponse> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse { Id = 1 });
        };

        // Act
        try
        {
            await behavior.Handle(request, next, CancellationToken.None);
        }
        catch (ValidatorException)
        {
            // Expected
        }

        // Assert
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationTokenToValidators()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        CancellationToken capturedToken = default;

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()))
            .Callback<IValidationContext, CancellationToken>((_, token) => capturedToken = token)
            .ReturnsAsync(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "test" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(new TestResponse { Id = 1 });

        // Act
        await behavior.Handle(request, next, cts.Token);

        // Assert
        capturedToken.Should().Be(cts.Token);
    }

    [Fact]
    public async Task Handle_ValidationErrorMessage_ShouldIncludePropertyNameAndMessage()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("PropertyName", "Error message")
            }));

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var request = new TestRequest { Name = "" };

        RequestHandlerDelegate<TestResponse> next = (ct) => Task.FromResult(new TestResponse { Id = 1 });

        // Act
        var act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidatorException>();
        exception.Which.Message.Should().Contain("PropertyName: Error message");
    }

    // Test request and response classes
    public class TestRequest : IRequest<TestResponse>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public int Id { get; set; }
    }
}
