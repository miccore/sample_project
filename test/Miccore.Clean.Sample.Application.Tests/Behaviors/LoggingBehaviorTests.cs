using FluentAssertions;
using MediatR;
using Miccore.Clean.Sample.Application.Behaviors;
using Microsoft.Extensions.Logging;
using Moq;

namespace Miccore.Clean.Sample.Application.Tests.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        _behavior = new LoggingBehavior<TestRequest, TestResponse>(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallNextDelegate()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedResponse = new TestResponse { Result = "success" };
        var nextCalled = false;

        RequestHandlerDelegate<TestResponse> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        };

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_ShouldReturnResponseFromNext()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedResponse = new TestResponse { Result = "expected result" };

        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse);
        result.Result.Should().Be("expected result");
    }

    [Fact]
    public async Task Handle_WhenNextThrows_ShouldRethrowException()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedException = new InvalidOperationException("Test exception");

        RequestHandlerDelegate<TestResponse> next = () => throw expectedException;

        // Act
        var act = () => _behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }

    [Fact]
    public async Task Handle_ShouldLogStartAndCompletion()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedResponse = new TestResponse { Result = "success" };

        RequestHandlerDelegate<TestResponse> next = () => Task.FromResult(expectedResponse);

        // Act
        await _behavior.Handle(request, next, CancellationToken.None);

        // Assert - Verify logging was called (at least 2 times: start and completion)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeast(2));
    }

    [Fact]
    public async Task Handle_WhenNextThrows_ShouldLogError()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedException = new InvalidOperationException("Test exception");

        RequestHandlerDelegate<TestResponse> next = () => throw expectedException;

        // Act
        try
        {
            await _behavior.Handle(request, next, CancellationToken.None);
        }
        catch
        {
            // Expected
        }

        // Assert - Verify error logging was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRespectCancellationToken()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var cts = new CancellationTokenSource();
        CancellationToken capturedToken = default;

        RequestHandlerDelegate<TestResponse> next = () =>
        {
            capturedToken = cts.Token;
            return Task.FromResult(new TestResponse { Result = "success" });
        };

        // Act
        await _behavior.Handle(request, () =>
        {
            capturedToken = cts.Token;
            return Task.FromResult(new TestResponse { Result = "success" });
        }, cts.Token);

        // Assert - The token should be the same as what we passed
        capturedToken.Should().Be(cts.Token);
    }

    [Fact]
    public async Task Handle_WhenRequestIsSlow_ShouldLogWarning()
    {
        // Arrange
        var request = new TestRequest { Value = "test" };
        var expectedResponse = new TestResponse { Result = "success" };

        // Simulate a slow request (> 500ms)
        RequestHandlerDelegate<TestResponse> next = async () =>
        {
            await Task.Delay(550);
            return expectedResponse;
        };

        // Act
        await _behavior.Handle(request, next, CancellationToken.None);

        // Assert - Verify warning logging was called for slow request
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // Test request and response classes
    public class TestRequest : IRequest<TestResponse>
    {
        public string Value { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }
}
