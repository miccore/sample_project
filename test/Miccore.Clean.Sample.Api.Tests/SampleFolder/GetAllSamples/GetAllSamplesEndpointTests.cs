using System.Net;
using FluentAssertions;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Sample.Endpoints.GetAllSamples;
using Miccore.Clean.Sample.Application.Sample.Queries.GetAllSamples;
using Miccore.Clean.Sample.Application.Sample.Responses;
using FastEndpoints;
using Miccore.Pagination.Model;

namespace Miccore.Clean.Sample.Api.Tests.Sample.GetSamples
{
    public class GetAllSamplesEndpointTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetAllSamplesEndpoint _endpoint;

        public GetAllSamplesEndpointTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _endpoint = Factory.Create<GetAllSamplesEndpoint>(_mediatorMock.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnSamplesResponse_WhenRequestIsValidAndPaginate()
        {
            // Arrange
            var request = new GetAllSamplesRequest { paginate = true };
            var sampleResponse = new PaginationModel<SampleResponse>
            {
                Items = new List<SampleResponse>
                {
                    new SampleResponse { Id = Guid.NewGuid(), Name = "Sample 1" },
                    new SampleResponse { Id = Guid.NewGuid(), Name = "Sample 2" }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sampleResponse);

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _endpoint.Response.Data.Should().NotBeNull();
            _endpoint.Response.Data.Should().BeOfType<PaginationModel<GetAllSamplesResponse>>();
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnSamplesResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new GetAllSamplesRequest { paginate = false };
            var sampleResponse = new PaginationModel<SampleResponse>
            {
                Items = new List<SampleResponse>
                {
                    new SampleResponse { Id = Guid.NewGuid(), Name = "Sample 1" },
                    new SampleResponse { Id = Guid.NewGuid(), Name = "Sample 2" }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sampleResponse);

            // Act
            await _endpoint.HandleAsync(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _endpoint.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _endpoint.Response.Data.Should().NotBeNull();
            _endpoint.Response.Data.Should().BeOfType<PaginationModel<GetAllSamplesResponse>>();
        }

        [Fact]
        public async Task HandleAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new GetAllSamplesRequest { paginate = false };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
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