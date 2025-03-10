using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Sample.Endpoints.UpdateSample;
using Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using FastEndpoints;

namespace Miccore.Clean.Sample.Api.Tests.Sample.UpdateSample;

public class UpdateSampleEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UpdateSampleEndpoint _endpoint;

    public UpdateSampleEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = Factory.Create<UpdateSampleEndpoint>(_mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnUpdatedResponse_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "Updated Sample" };
        var sampleResponse = new SampleResponse { Id = Guid.NewGuid(), Name = "Test Sample" };
        var updateSampleResponse = new UpdateSampleResponse { Id = request.Id, Name = "Updated Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sampleResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        _endpoint.Response.Data.Should().NotBeNull();
        _endpoint.Response.Data.Should().BeOfType<UpdateSampleResponse>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "" };
        _endpoint.ValidationFailures.Add(new ValidationFailure("Name", "Name is required"));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        _endpoint.Response.Errors.Should().NotBeNullOrEmpty();
        _endpoint.Response.Errors.Count().Should().Be(1);
        _ = _endpoint.Response.Errors.First().Message.Should().Be("Name: Name is required");
    } 

    [Fact]
    public async Task HandleAsync_ShouldReturnNotFound_WhenSampleNotFound()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "Updated Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Sample not found"));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        _endpoint.Response.Errors.Should().NotBeNullOrEmpty();
        _endpoint.Response.Errors.Count().Should().Be(1);
        _ = _endpoint.Response.Errors.First().Message.Should().Be("Sample not found");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "Updated Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _endpoint.Response.Errors.Should().NotBeNullOrEmpty();
        _endpoint.Response.Errors.Count().Should().Be(1);
        _ = _endpoint.Response.Errors.First().Message.Should().Be("Internal server error");
    }
}