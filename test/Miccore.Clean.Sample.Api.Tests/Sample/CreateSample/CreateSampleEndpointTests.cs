using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Sample.Endpoints.CreateSample;
using Miccore.Clean.Sample.Application.Sample.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using FastEndpoints;

namespace Miccore.Clean.Sample.Api.Tests.Sample.CreateSample
{
    public class CreateSampleEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateSampleEndpoint _endpoint;

        public CreateSampleEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = Factory.Create<CreateSampleEndpoint>(_mediatorMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnCreatedResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateSampleRequest { Name = "Test Sample" };
            var sampleResponse = new SampleResponse { Id = Guid.NewGuid(), Name = "Test Sample" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSampleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sampleResponse);

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<CreateSampleCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Created);
            _endpoint.Response.Data.Should().NotBeNull();
            _endpoint.Response.Data.Should().BeOfType<CreateSampleResponse>();
            _endpoint.Response.Data.Id.Should().Be(sampleResponse.Id);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new CreateSampleRequest { Name = "" }; 
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
            var request = new CreateSampleRequest { Name = "Test Sample" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSampleCommand>(), It.IsAny<CancellationToken>()))
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
            var request = new CreateSampleRequest { Name = "Test Sample" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSampleCommand>(), It.IsAny<CancellationToken>()))
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
}