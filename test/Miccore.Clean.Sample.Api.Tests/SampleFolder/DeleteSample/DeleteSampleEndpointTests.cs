using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Sample.Endpoints.DeleteSample;
using Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Application.Sample.Responses;
using FastEndpoints;

namespace Miccore.Clean.Sample.Api.Tests.Sample.DeleteSample
{
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
        public async Task HandleAsync_ShouldReturnNoContent_WhenRequestIsValid()
        {
            // Arrange
            var request = new DeleteSampleRequest(Guid.NewGuid());

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SampleResponse());

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new DeleteSampleRequest(Guid.NewGuid());
            _endpoint.ValidationFailures.Add(new ValidationFailure("Id", "Id is required"));

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnNotFound_WhenSampleNotFound()
        {
            // Arrange
            var request = new DeleteSampleRequest(Guid.NewGuid());

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Sample not found"));

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new DeleteSampleRequest(Guid.NewGuid());

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSampleCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}