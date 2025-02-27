using Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Queries;

public class GetSampleByIdQueryHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly GetSampleByIdQueryHandler _handler;

    public GetSampleByIdQueryHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _handler = new GetSampleByIdQueryHandler(_sampleRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSampleResponse_WhenSampleExists()
    {
        // Arrange
        var query = new GetSampleByIdQuery(Guid.NewGuid());
        var sampleEntity = new SampleEntity { Id = query.Id, Name = "Test Sample" };
        var sampleResponse = new SampleResponse { Id = sampleEntity.Id, Name = "Test Sample" };

        _sampleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(sampleEntity);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sampleResponse.Id);
        result.Name.Should().Be(sampleResponse.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSampleDoesNotExist()
    {
        // Arrange
        var query = new GetSampleByIdQuery(Guid.NewGuid());

        _sampleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new NotFoundException("Sample not found"));

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}
