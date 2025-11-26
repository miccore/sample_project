using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Features.Samples.GetSample;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using FastEndpoints;

namespace Miccore.Clean.Sample.Api.Tests.Features.Samples.GetSample;

public class GetSampleEndpointTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetSampleEndpoint _endpoint;

    public GetSampleEndpointTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _endpoint = Factory.Create<GetSampleEndpoint>(_mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var sampleId = Guid.NewGuid();
        var request = new GetSampleRequest(sampleId);
        var sampleResponse = new SampleResponse { Id = sampleId, Name = "Test Sample" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSampleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sampleResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetSampleQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        _endpoint.Response.Data.Should().NotBeNull();
        _endpoint.Response.Data!.Id.Should().Be(sampleId);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowValidatorException_WhenValidationFails()
    {
        // Arrange
        var request = new GetSampleRequest(Guid.Empty);
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
        var request = new GetSampleRequest(Guid.NewGuid());

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSampleQuery>(), It.IsAny<CancellationToken>()))
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
        var request = new GetSampleRequest(Guid.NewGuid());

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetSampleQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Internal server error");
    }
}
