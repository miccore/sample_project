using FastEndpoints;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Miccore.Clean.Sample.Api.Features.Samples.DeleteSample;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using Moq;

namespace Miccore.Clean.Sample.Api.Tests.Features.Samples.DeleteSample;

public class DeleteSampleEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteSampleEndpoint _endpoint;

    public DeleteSampleEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = Factory.Create<DeleteSampleEndpoint>(_mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var sampleId = Guid.NewGuid();
        var request = new DeleteSampleRequest(sampleId);
        var sampleResponse = new SampleResponse { Id = sampleId, Name = "Deleted Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sampleResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowValidatorException_WhenValidationFails()
    {
        // Arrange
        var request = new DeleteSampleRequest(Guid.Empty);
        _endpoint.ValidationFailures.Add(new ValidationFailure("Id", "Id is required"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<ValidatorException>()
            .WithMessage("*Id: Id is required*");
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFoundException_WhenSampleNotFound()
    {
        // Arrange
        var request = new DeleteSampleRequest(Guid.NewGuid());

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()))
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
        var request = new DeleteSampleRequest(Guid.NewGuid());

        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Internal server error");
    }
}
