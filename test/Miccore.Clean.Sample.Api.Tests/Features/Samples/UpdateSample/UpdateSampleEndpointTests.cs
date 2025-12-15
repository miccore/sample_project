using FastEndpoints;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Miccore.Clean.Sample.Api.Features.Samples.UpdateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using Moq;

namespace Miccore.Clean.Sample.Api.Tests.Features.Samples.UpdateSample;

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
    public async Task HandleAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "Updated Sample" };
        var sampleResponse = new SampleResponse { Id = request.Id, Name = "Updated Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sampleResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _endpoint.Response.Data.Should().NotBeNull();
        _endpoint.Response.Data!.Id.Should().Be(sampleResponse.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowValidatorException_WhenValidationFails()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "" };
        _endpoint.ValidationFailures.Add(new ValidationFailure("Name", "Name is required"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<ValidatorException>()
            .WithMessage("*Name: Name is required*");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenSampleNotFound()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "Updated Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Sample not found"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Sample not found");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var request = new UpdateSampleRequest { Id = Guid.NewGuid(), Name = "Updated Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSampleCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Internal server error");
    }
}
