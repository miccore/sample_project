using FluentAssertions;
using MediatR;
using Moq;
using Miccore.Clean.Sample.Api.Features.Samples.GetAllSamples;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Pagination;
using FastEndpoints;

namespace Miccore.Clean.Sample.Api.Tests.Features.Samples.GetAllSamples;

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
    public async Task HandleAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        // Arrange
        var request = new GetAllSamplesRequest { Page = 1, Limit = 10 };
        var samples = new List<SampleResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "Sample 1" },
            new() { Id = Guid.NewGuid(), Name = "Sample 2" }
        };
        var paginationResponse = new PaginationModel<SampleResponse>
        {
            Items = samples,
            TotalItems = 2,
            CurrentPage = 1,
            TotalPages = 1
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        _endpoint.Response.Data.Should().NotBeNull();
        _endpoint.Response.Data!.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoSamplesExist()
    {
        // Arrange
        var request = new GetAllSamplesRequest { Page = 1, Limit = 10 };
        var paginationResponse = new PaginationModel<SampleResponse>
        {
            Items = new List<SampleResponse>(),
            TotalItems = 0,
            CurrentPage = 1,
            TotalPages = 0
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _endpoint.Response.Data!.Items.Should().BeEmpty();
        _endpoint.Response.Data!.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenExceptionOccurs()
    {
        // Arrange
        var request = new GetAllSamplesRequest { Page = 1, Limit = 10 };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act & Assert
        var act = () => _endpoint.HandleAsync(request, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Internal server error");
    }

    [Fact]
    public async Task HandleAsync_WithPagination_ShouldAddRouteLinks()
    {
        // Arrange
        var request = new GetAllSamplesRequest { Page = 1, Limit = 10, Paginate = true };
        var samples = new List<SampleResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "Sample 1" },
            new() { Id = Guid.NewGuid(), Name = "Sample 2" }
        };
        var paginationResponse = new PaginationModel<SampleResponse>
        {
            Items = samples,
            TotalItems = 20,
            CurrentPage = 1,
            TotalPages = 2
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginationResponse);

        // Act
        await _endpoint.HandleAsync(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllSamplesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        _endpoint.Response.Data.Should().NotBeNull();
        _endpoint.Response.Data!.TotalPages.Should().Be(2);
    }
}
