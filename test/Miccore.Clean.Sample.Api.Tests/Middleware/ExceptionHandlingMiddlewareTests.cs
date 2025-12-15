using System.Net;
using FluentAssertions;
using Miccore.Clean.Sample.Api.Middleware;
using Miccore.Clean.Sample.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;

namespace Miccore.Clean.Sample.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly Mock<IHostEnvironment> _environmentMock;

    public ExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _environmentMock = new Mock<IHostEnvironment>();
    }

    private ExceptionHandlingMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/test";
        context.Items["CorrelationId"] = "test-correlation-id";
        return context;
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_ShouldReturn404()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new NotFoundException("Sample with id 123 not found");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        context.Response.ContentType.Should().Be("application/problem+json");

        var responseBody = await GetResponseBody(context);
        responseBody.Should().Contain("Not Found");
    }

    [Fact]
    public async Task InvokeAsync_WhenValidatorException_ShouldReturn400()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new ValidatorException("Name is required");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        var responseBody = await GetResponseBody(context);
        responseBody.Should().Contain("Name is required");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedAccessException_ShouldReturn401()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new UnauthorizedAccessException();

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WhenArgumentException_ShouldReturn400()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new ArgumentException("Invalid argument");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenOperationCanceledException_ShouldReturn400()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new OperationCanceledException();

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenGenericException_ShouldReturn500()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new Exception("Unexpected error");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task InvokeAsync_InProduction_ShouldNotExposeInternalErrorDetails()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");

        RequestDelegate next = _ => throw new Exception("Sensitive internal error");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var responseBody = await GetResponseBody(context);
        responseBody.Should().NotContain("Sensitive internal error");
        responseBody.Should().Contain("unexpected error");
    }

    [Fact]
    public async Task InvokeAsync_InDevelopment_ShouldIncludeStackTraceForServerErrors()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new Exception("Test error");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var responseBody = await GetResponseBody(context);
        responseBody.Should().Contain("stackTrace");
    }

    [Fact]
    public async Task InvokeAsync_ShouldIncludeCorrelationIdInResponse()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new NotFoundException("Sample with id 123 not found");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var responseBody = await GetResponseBody(context);
        responseBody.Should().Contain("correlationId");
        responseBody.Should().Contain("test-correlation-id");
    }

    [Fact]
    public async Task InvokeAsync_WhenNoCorrelationId_ShouldUseNA()
    {
        // Arrange
        _environmentMock.Setup(e => e.EnvironmentName).Returns("Development");

        RequestDelegate next = _ => throw new NotFoundException("Sample with id 123 not found");

        var middleware = CreateMiddleware(next);
        var context = CreateHttpContext();
        context.Items.Remove("CorrelationId");

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var responseBody = await GetResponseBody(context);
        responseBody.Should().Contain("N/A");
    }

    private static async Task<string> GetResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}
