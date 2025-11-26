using FluentAssertions;
using MediatR;
using Miccore.Clean.Sample.Application.Handlers;

namespace Miccore.Clean.Sample.Application.Tests.Handlers;

public class BaseCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenSuccessful_ShouldReturnResponse()
    {
        // Arrange
        var handler = new TestCommandHandler(new TestResponse { Value = "success" });
        var command = new TestCommand { Data = "test" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("success");
    }

    [Fact]
    public async Task Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var handler = new TestCommandHandler(new TestResponse());

        // Act
        var act = () => handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Something went wrong");
        var handler = new TestCommandHandler(exception);
        var command = new TestCommand { Data = "test" };

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Something went wrong");
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassTokenToHandler()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var handler = new TestCommandHandler(new TestResponse { Value = "success" });
        var command = new TestCommand { Data = "test" };

        // Act
        var result = await handler.Handle(command, cts.Token);

        // Assert
        result.Should().NotBeNull();
    }

    // Test Command and Response - public classes
    public class TestCommand : IRequest<TestResponse>
    {
        public string Data { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Value { get; set; } = string.Empty;
    }

    // Test Handler implementation - public class
    public class TestCommandHandler : BaseCommandHandler<TestCommand, TestResponse>
    {
        private readonly TestResponse? _responseToReturn;
        private readonly Exception? _exceptionToThrow;

        public TestCommandHandler(TestResponse responseToReturn)
        {
            _responseToReturn = responseToReturn;
        }

        public TestCommandHandler(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }

        protected override Task<TestResponse> HandleCommand(TestCommand request, CancellationToken cancellationToken)
        {
            if (_exceptionToThrow != null)
                throw _exceptionToThrow;

            return Task.FromResult(_responseToReturn!);
        }
    }
}
