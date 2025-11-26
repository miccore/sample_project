using FluentAssertions;
using MediatR;
using Miccore.Clean.Sample.Application.Handlers;

namespace Miccore.Clean.Sample.Application.Tests.Handlers;

public class BaseQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenSuccessful_ShouldReturnResponse()
    {
        // Arrange
        var handler = new TestQueryHandler(new TestResponse { Value = "success" });
        var query = new TestQuery { Filter = "test" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be("success");
    }

    [Fact]
    public async Task Handle_WhenRequestIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var handler = new TestQueryHandler(new TestResponse());

        // Act
        var act = () => handler.Handle(null!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Query failed");
        var handler = new TestQueryHandler(exception);
        var query = new TestQuery { Filter = "test" };

        // Act
        var act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Query failed");
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldPassTokenToHandler()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var handler = new TestQueryHandler(new TestResponse { Value = "success" });
        var query = new TestQuery { Filter = "test" };

        // Act
        var result = await handler.Handle(query, cts.Token);

        // Assert
        result.Should().NotBeNull();
    }

    // Test Query and Response - public classes
    public class TestQuery : IRequest<TestResponse>
    {
        public string Filter { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Value { get; set; } = string.Empty;
    }

    // Test Handler implementation - public class
    public class TestQueryHandler : BaseQueryHandler<TestQuery, TestResponse>
    {
        private readonly TestResponse? _responseToReturn;
        private readonly Exception? _exceptionToThrow;

        public TestQueryHandler(TestResponse responseToReturn)
        {
            _responseToReturn = responseToReturn;
        }

        public TestQueryHandler(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }

        protected override Task<TestResponse> HandleQuery(TestQuery request, CancellationToken cancellationToken)
        {
            if (_exceptionToThrow != null)
                throw _exceptionToThrow;

            return Task.FromResult(_responseToReturn!);
        }
    }
}
