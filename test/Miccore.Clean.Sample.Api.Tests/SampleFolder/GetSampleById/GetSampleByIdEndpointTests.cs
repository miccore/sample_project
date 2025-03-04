using System.Net;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Sample.Endpoints.GetSampleById;
using Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using FastEndpoints;

namespace Miccore.Clean.Sample.Api.Tests.Sample.GetSampleById
{
    public class GetSampleByIdEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetSampleByIdEndpoint _endpoint;

        public GetSampleByIdEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = Factory.Create<GetSampleByIdEndpoint>(_mediatorMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnSampleResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new GetSampleByIdRequest(Guid.NewGuid());
            var sampleResponse = new SampleResponse { Id = request.Id, Name = "Test Sample" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSampleByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sampleResponse);

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetSampleByIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _endpoint.Response.Data.Should().NotBeNull();
            _endpoint.Response.Data.Should().BeOfType<GetSampleByIdSampleResponse>();
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new GetSampleByIdRequest(Guid.NewGuid());
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
            var request = new GetSampleByIdRequest(Guid.NewGuid());

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSampleByIdQuery>(), It.IsAny<CancellationToken>()))
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
            var request = new GetSampleByIdRequest(Guid.NewGuid());

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSampleByIdQuery>(), It.IsAny<CancellationToken>()))
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