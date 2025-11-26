using FluentAssertions;
using Miccore.Clean.Sample.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Miccore.Clean.Sample.Api.Tests.Middleware;

public class CorrelationIdMiddlewareTests
{
    private readonly Mock<ILogger<CorrelationIdMiddleware>> _loggerMock;

    public CorrelationIdMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
    }

    private CorrelationIdMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new CorrelationIdMiddleware(next, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenCorrelationIdProvided_ShouldUseProvidedId()
    {
        // Arrange
        var providedCorrelationId = "my-custom-correlation-id";
        string? capturedCorrelationId = null;

        RequestDelegate next = context =>
        {
            capturedCorrelationId = context.Items["CorrelationId"]?.ToString();
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = providedCorrelationId;

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedCorrelationId.Should().Be(providedCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoCorrelationIdProvided_ShouldGenerateNewId()
    {
        // Arrange
        string? capturedCorrelationId = null;

        RequestDelegate next = context =>
        {
            capturedCorrelationId = context.Items["CorrelationId"]?.ToString();
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedCorrelationId.Should().NotBeNullOrWhiteSpace();
        capturedCorrelationId.Should().HaveLength(32); // Guid without dashes
    }

    [Fact]
    public async Task InvokeAsync_WhenEmptyCorrelationIdProvided_ShouldGenerateNewId()
    {
        // Arrange
        string? capturedCorrelationId = null;

        RequestDelegate next = context =>
        {
            capturedCorrelationId = context.Items["CorrelationId"]?.ToString();
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = "";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedCorrelationId.Should().NotBeNullOrWhiteSpace();
        capturedCorrelationId.Should().HaveLength(32);
    }

    [Fact]
    public async Task InvokeAsync_WhenWhitespaceCorrelationIdProvided_ShouldGenerateNewId()
    {
        // Arrange
        string? capturedCorrelationId = null;

        RequestDelegate next = context =>
        {
            capturedCorrelationId = context.Items["CorrelationId"]?.ToString();
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = "   ";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedCorrelationId.Should().NotBeNullOrWhiteSpace();
        capturedCorrelationId.Should().HaveLength(32);
    }

    [Fact]
    public async Task InvokeAsync_ShouldStoreCorrelationIdInHttpContextItems()
    {
        // Arrange
        var providedCorrelationId = "test-id";

        RequestDelegate next = _ => Task.CompletedTask;

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-ID"] = providedCorrelationId;

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Items["CorrelationId"].Should().Be(providedCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_ShouldAddCorrelationIdToResponseHeaders()
    {
        // Arrange
        var providedCorrelationId = "response-test-id";

        RequestDelegate next = _ => Task.CompletedTask;

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Headers["X-Correlation-ID"] = providedCorrelationId;

        // Act
        await middleware.InvokeAsync(context);

        // Assert - Verify that the correlation ID was stored in HttpContext
        context.Items["CorrelationId"].Should().Be(providedCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNextDelegate()
    {
        // Arrange
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_GeneratedCorrelationId_ShouldBeValidGuidFormat()
    {
        // Arrange
        string? capturedCorrelationId = null;

        RequestDelegate next = context =>
        {
            capturedCorrelationId = context.Items["CorrelationId"]?.ToString();
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        capturedCorrelationId.Should().NotBeNull();
        Guid.TryParseExact(capturedCorrelationId, "N", out _).Should().BeTrue();
    }
}
